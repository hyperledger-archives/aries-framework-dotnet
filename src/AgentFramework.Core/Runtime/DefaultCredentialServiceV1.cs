using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators.Attachments;
using AgentFramework.Core.Decorators.Threading;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages.Credentials.V1;
using AgentFramework.Core.Models.Credentials;
using AgentFramework.Core.Models.Events;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.BlobStorageApi;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public partial class DefaultCredentialService : ICredentialService
    {
        /// <inheritdoc />
        public async Task<string> ProcessOfferAsync(IAgentContext agentContext, CredentialOfferMessage credentialOffer, ConnectionRecord connection)
        {
            var offerAttachment = credentialOffer.Offers.FirstOrDefault(x => x.Id == "libindy-cred-offer-0")
                ?? throw new ArgumentNullException(nameof(CredentialOfferMessage.Offers));

            var offerJson = offerAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();
            var offer = JObject.Parse(offerJson);
            var definitionId = offer["cred_def_id"].ToObject<string>();
            var schemaId = offer["schema_id"].ToObject<string>();

            // Write offer record to local wallet
            var credentialRecord = new CredentialRecord
            {
                Id = Guid.NewGuid().ToString(),
                OfferJson = offerJson,
                ConnectionId = connection.Id,
                CredentialDefinitionId = definitionId,
                CredentialAttributesValues = credentialOffer.CredentialPreview?.Attributes
                    .Select(x => new CredentialPreviewAttribute
                    {
                        Name = x.Name,
                        MimeType = x.MimeType,
                        Value = x.Value
                    }).ToArray(),
                SchemaId = schemaId,
                State = CredentialState.Offered
            };
            credentialRecord.SetTag(TagConstants.Role, TagConstants.Holder);
            credentialRecord.SetTag(TagConstants.LastThreadId, credentialOffer.GetThreadId());

            await RecordService.AddAsync(agentContext.Wallet, credentialRecord);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = credentialOffer.Type,
                ThreadId = credentialOffer.GetThreadId()
            });

            return credentialRecord.Id;
        }

        public async Task<(CredentialRequestMessage, CredentialRecord)> CreateRequestAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credential.State}'");

            var connection = await ConnectionService.GetAsync(agentContext, credential.ConnectionId);

            var definition =
                await LedgerService.LookupDefinitionAsync(await agentContext.Pool, credential.CredentialDefinitionId);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            var request = await AnonCreds.ProverCreateCredentialReqAsync(agentContext.Wallet, connection.MyDid,
                credential.OfferJson,
                definition.ObjectJson, provisioning.MasterSecretId);

            // Update local credential record with new info
            credential.CredentialRequestMetadataJson = request.CredentialRequestMetadataJson;

            await credential.TriggerAsync(CredentialTrigger.Request);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);

            var threadId = credential.GetTag(TagConstants.LastThreadId);
            var response = new CredentialRequestMessage
            {
                Requests = new[] { 
                    new Attachment
                {
                    Id = "libindy-cred-request-0",
                    MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                    Data = new AttachmentContent
                    {
                        Base64 = request.CredentialRequestJson.GetUTF8Bytes().ToBase64String()
                    }
                } }
            };

            response.ThreadFrom(threadId);
            return (response, credential);
        }

        public async Task<string> ProcessCredentialAsync(IAgentContext agentContext, CredentialIssueMessage credential, ConnectionRecord connection)
        {
            var credentialAttachment = credential.Credentials.FirstOrDefault(x => x.Id == "libindy-cred-0")
                ?? throw new ArgumentException("Credential attachment not found");

            var credentialJson = credentialAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();
            var credentialJobj = JObject.Parse(credentialJson);
            var definitionId = credentialJobj["cred_def_id"].ToObject<string>();
            var revRegId = credentialJobj["rev_reg_id"]?.ToObject<string>();

            var credentialRecord = await this.GetByThreadIdAsync(agentContext, credential.GetThreadId());

            if (credentialRecord.State != CredentialState.Requested)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Requested}', found '{credentialRecord.State}'");

            var credentialDefinition = await LedgerService.LookupDefinitionAsync(await agentContext.Pool, definitionId);

            string revocationRegistryDefinitionJson = null;
            if (!string.IsNullOrEmpty(revRegId))
            {
                // If credential supports revocation, lookup registry definition
                var revocationRegistry =
                    await LedgerService.LookupRevocationRegistryDefinitionAsync(await agentContext.Pool, revRegId);
                revocationRegistryDefinitionJson = revocationRegistry.ObjectJson;
            }

            var credentialId = await AnonCreds.ProverStoreCredentialAsync(
                wallet: agentContext.Wallet,
                credId: null,
                credReqMetadataJson: credentialRecord.CredentialRequestMetadataJson,
                credJson: credentialJson,
                credDefJson: credentialDefinition.ObjectJson,
                revRegDefJson: revocationRegistryDefinitionJson);

            credentialRecord.CredentialId = credentialId;
            credentialRecord.CredentialAttributesValues = null;

            await credentialRecord.TriggerAsync(CredentialTrigger.Issue);
            await RecordService.UpdateAsync(agentContext.Wallet, credentialRecord);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = credential.Type,
                ThreadId = credential.GetThreadId()
            });

            return credentialRecord.Id;
        }

        public async Task<(CredentialOfferMessage, CredentialRecord)> CreateOfferV1Async(IAgentContext agentContext, OfferConfiguration config, string connectionId = null)
        {
            Logger.LogInformation(LoggingEvents.CreateCredentialOffer, "DefinitionId {0}, IssuerDid {1}",
                config.CredentialDefinitionId, config.IssuerDid);

            var threadId = Guid.NewGuid().ToString();

            if (!string.IsNullOrEmpty(connectionId))
            {
                var connection = await ConnectionService.GetAsync(agentContext, connectionId);

                if (connection.State != ConnectionState.Connected)
                {
                    throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                        $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");
                }
            }

            if (config.CredentialAttributeValues != null && config.CredentialAttributeValues.Any())
            {
                CredentialUtils.ValidateCredentialPreviewAttributes(config.CredentialAttributeValues);
            }

            var offerJson = await AnonCreds.IssuerCreateCredentialOfferAsync(
                agentContext.Wallet, config.CredentialDefinitionId);
            var offerJobj = JObject.Parse(offerJson);
            var schemaId = offerJobj["schema_id"].ToObject<string>();

            // Write offer record to local wallet
            var credentialRecord = new CredentialRecord
            {
                Id = Guid.NewGuid().ToString(),
                CredentialDefinitionId = config.CredentialDefinitionId,
                OfferJson = offerJson,
                ConnectionId = connectionId,
                SchemaId = schemaId,
                CredentialAttributesValues = config.CredentialAttributeValues,
                State = CredentialState.Offered,
            };

            credentialRecord.SetTag(TagConstants.LastThreadId, threadId);
            credentialRecord.SetTag(TagConstants.Role, TagConstants.Issuer);

            if (!string.IsNullOrEmpty(config.IssuerDid))
                credentialRecord.SetTag(TagConstants.IssuerDid, config.IssuerDid);

            if (config.Tags != null)
                foreach (var tag in config.Tags)
                {
                    if (!credentialRecord.Tags.Keys.Contains(tag.Key))
                        credentialRecord.Tags.Add(tag.Key, tag.Value);
                }

            await RecordService.AddAsync(agentContext.Wallet, credentialRecord);

            return (new CredentialOfferMessage
            {
                Id = threadId,
                Offers = new Attachment[]
                {
                    new Attachment
                    {
                        Id = "libindy-cred-offer-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = offerJson.GetUTF8Bytes().ToBase64String()
                        }
                    }
                },
                CredentialPreview = credentialRecord.CredentialAttributesValues != null ? new CredentialPreviewMessage
                {
                    Attributes = credentialRecord.CredentialAttributesValues.Select(x => new CredentialPreviewAttriubute
                    {
                        Name = x.Name,
                        MimeType = x.MimeType,
                        Value = x.Value?.ToString()
                    }).ToArray()
                } : null
            }, credentialRecord);
        }

        public async Task<string> ProcessCredentialRequestAsync(IAgentContext agentContext, Messages.Credentials.V1.CredentialRequestMessage credentialRequest, ConnectionRecord connection)
        {
            Logger.LogInformation(LoggingEvents.StoreCredentialRequest, "Type {0},", credentialRequest.Type);

            // TODO Handle case when no thread is included
            var credential = await this.GetByThreadIdAsync(agentContext, credentialRequest.GetThreadId());

            var credentialAttachment = credentialRequest.Requests.FirstOrDefault(x => x.Id == "libindy-cred-request-0")
                ?? throw new ArgumentException("Credential request attachment not found.");

            if (credential.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credential.State}'");

            credential.RequestJson = credentialAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();
            credential.ConnectionId = connection.Id;

            await credential.TriggerAsync(CredentialTrigger.Request);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credential.Id,
                MessageType = credentialRequest.Type,
                ThreadId = credentialRequest.GetThreadId()
            });

            return credential.Id;
        }

        public Task<(CredentialIssueMessage, CredentialRecord)> CreateCredentialAsync(IAgentContext agentContext, string credentialId)
        {
            return CreateCredentialAsync(agentContext, credentialId, values: null);
        }

        public async Task<(CredentialIssueMessage, CredentialRecord)> CreateCredentialAsync(IAgentContext agentContext, string credentialId, IEnumerable<CredentialPreviewAttribute> values)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Requested)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Requested}', found '{credential.State}'");

            if (values != null && values.Any())
                credential.CredentialAttributesValues = values;

            var definitionRecord =
                await SchemaService.GetCredentialDefinitionAsync(agentContext.Wallet, credential.CredentialDefinitionId);

            var connection = await ConnectionService.GetAsync(agentContext, credential.ConnectionId);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            if (connection.State != ConnectionState.Connected)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");

            string revocationRegistryId = null;
            BlobStorageReader tailsReader = null;
            if (definitionRecord.SupportsRevocation)
            {
                var revocationRecordSearch = await RecordService.SearchAsync<RevocationRegistryRecord>(
                    agentContext.Wallet, SearchQuery.Equal(nameof(RevocationRegistryRecord.CredentialDefinitionId), definitionRecord.Id), null, 5);

                var revocationRecord = revocationRecordSearch.Single(); // TODO: Credential definition can have multiple revocation registries

                revocationRegistryId = revocationRecord.Id;
                tailsReader = await TailsService.OpenTailsAsync(revocationRecord.TailsFile);
            }

            var issuedCredential = await AnonCreds.IssuerCreateCredentialAsync(agentContext.Wallet, credential.OfferJson,
                credential.RequestJson, CredentialUtils.FormatCredentialValues(credential.CredentialAttributesValues), revocationRegistryId, tailsReader);

            if (definitionRecord.SupportsRevocation)
            {
                var paymentInfo = await PaymentService.GetTransactionCostAsync(agentContext, TransactionTypes.REVOC_REG_ENTRY);

                await LedgerService.SendRevocationRegistryEntryAsync(
                    context: agentContext,
                    issuerDid: provisioning.IssuerDid,
                    revocationRegistryDefinitionId: revocationRegistryId,
                    revocationDefinitionType: "CL_ACCUM",
                    value: issuedCredential.RevocRegDeltaJson,
                    paymentInfo: paymentInfo);
                credential.CredentialRevocationId = issuedCredential.RevocId;

                if (paymentInfo != null)
                {
                    await RecordService.UpdateAsync(agentContext.Wallet, paymentInfo.PaymentAddress);
                }
            }

            await credential.TriggerAsync(CredentialTrigger.Issue);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
            var threadId = credential.GetTag(TagConstants.LastThreadId);

            var credentialMsg = new CredentialIssueMessage
            {
                Credentials = new []
                {
                    new Attachment
                    {
                        Id = "libindy-cred-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = issuedCredential.CredentialJson
                                .GetUTF8Bytes()
                                .ToBase64String()
                        }
                    }
                }
            };

            credentialMsg.ThreadFrom(threadId);

            return (credentialMsg, credential);
        }
    }
}