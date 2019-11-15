﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators;
using AgentFramework.Core.Decorators.Attachments;
using AgentFramework.Core.Decorators.Service;
using AgentFramework.Core.Decorators.Threading;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
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
using AgentFramework.Core.Decorators.Transport;
using Hyperledger.Indy.DidApi;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public partial class DefaultCredentialService : ICredentialService
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        protected readonly IEventAggregator EventAggregator;

        /// <summary>
        /// The ledger service
        /// </summary>
        protected readonly ILedgerService LedgerService;
        /// <summary>
        /// The connection service
        /// </summary>
        protected readonly IConnectionService ConnectionService;
        /// <summary>
        /// The record service
        /// </summary>
        protected readonly IWalletRecordService RecordService;
        /// <summary>
        /// The schema service
        /// </summary>
        protected readonly ISchemaService SchemaService;
        /// <summary>
        /// The tails service
        /// </summary>
        protected readonly ITailsService TailsService;
        /// <summary>
        /// The provisioning service
        /// </summary>
        protected readonly IProvisioningService ProvisioningService;

        /// <summary>
        /// Payment Service
        /// </summary>
        protected readonly IPaymentService PaymentService;
        /// <summary>
        /// Message Service
        /// </summary>
        protected readonly IMessageService MessageService;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<DefaultCredentialService> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCredentialService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="ledgerService">The ledger service.</param>
        /// <param name="connectionService">The connection service.</param>
        /// <param name="recordService">The record service.</param>
        /// <param name="schemaService">The schema service.</param>
        /// <param name="tailsService">The tails service.</param>
        /// <param name="provisioningService">The provisioning service.</param>
        /// <param name="paymentService">The payment service.</param>
        /// <param name="messageService">The message service</param>
        /// <param name="logger">The logger.</param>
        public DefaultCredentialService(
            IEventAggregator eventAggregator,
            ILedgerService ledgerService,
            IConnectionService connectionService,
            IWalletRecordService recordService,
            ISchemaService schemaService,
            ITailsService tailsService,
            IProvisioningService provisioningService,
            IPaymentService paymentService,
            IMessageService messageService,
            ILogger<DefaultCredentialService> logger)
        {
            EventAggregator = eventAggregator;
            LedgerService = ledgerService;
            ConnectionService = connectionService;
            RecordService = recordService;
            SchemaService = schemaService;
            TailsService = tailsService;
            ProvisioningService = provisioningService;
            PaymentService = paymentService;
            this.MessageService = messageService;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<CredentialRecord> GetAsync(IAgentContext agentContext, string credentialId)
        {
            var record = await RecordService.GetAsync<CredentialRecord>(agentContext.Wallet, credentialId);

            if (record == null)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Credential record not found");

            return record;
        }

        /// <inheritdoc />
        public virtual Task<List<CredentialRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100) =>
            RecordService.SearchAsync<CredentialRecord>(agentContext.Wallet, query, null, count);

        /// <inheritdoc />
        public virtual async Task RejectOfferAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credential.State}'");

            await credential.TriggerAsync(CredentialTrigger.Reject);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
        }

        /// <inheritdoc />
        public async Task RevokeCredentialOfferAsync(IAgentContext agentContext, string offerId)
        {
            var credentialRecord = await GetAsync(agentContext, offerId);

            if (credentialRecord.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credentialRecord.State}'");

            await RecordService.DeleteAsync<ConnectionRecord>(agentContext.Wallet, offerId);
        }

        /// <inheritdoc />
        public virtual async Task RejectCredentialRequestAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Requested)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Requested}', found '{credential.State}'");

            await credential.TriggerAsync(CredentialTrigger.Reject);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
        }

        /// <inheritdoc />
        public virtual async Task RevokeCredentialAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Issued)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Requested}', found '{credential.State}'");

            var definition = await SchemaService.GetCredentialDefinitionAsync(agentContext.Wallet, credential.CredentialDefinitionId);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            // Check if the state machine is valid for revocation
            await credential.TriggerAsync(CredentialTrigger.Revoke);

            var revocationRecordSearch = await RecordService.SearchAsync<RevocationRegistryRecord>(
                agentContext.Wallet, SearchQuery.Equal(nameof(RevocationRegistryRecord.CredentialDefinitionId), definition.Id), null, 5);
            var revocationRecord = revocationRecordSearch.Single(); // TODO: Add support for multiple revocation registries

            // Revoke the credential
            var tailsReader = await TailsService.OpenTailsAsync(revocationRecord.TailsFile);
            var revocRegistryDeltaJson = await AnonCreds.IssuerRevokeCredentialAsync(agentContext.Wallet, tailsReader,
                revocationRecord.Id, credential.CredentialRevocationId);

            var paymentInfo = await PaymentService.GetTransactionCostAsync(agentContext, TransactionTypes.REVOC_REG_ENTRY);

            // Write the delta state on the ledger for the corresponding revocation registry
            await LedgerService.SendRevocationRegistryEntryAsync(context: agentContext,
                                                                 issuerDid: provisioning.IssuerDid,
                                                                 revocationRegistryDefinitionId: revocationRecord.Id,
                                                                 revocationDefinitionType: "CL_ACCUM",
                                                                 value: revocRegistryDeltaJson,
                                                                 paymentInfo: paymentInfo);

            if (paymentInfo != null)
            {
                await RecordService.UpdateAsync(agentContext.Wallet, paymentInfo.PaymentAddress);
            }

            // Update local credential record
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
        }

        /// <inheritdoc />
        public async Task DeleteCredentialAsync(IAgentContext agentContext, string credentialId)
        {
            var credentialRecord = await GetAsync(agentContext, credentialId);
            try
            {
                await AnonCreds.ProverDeleteCredentialAsync(agentContext.Wallet, credentialRecord.CredentialId);
            }
            catch
            {
                // OK
            }
            await RecordService.DeleteAsync<CredentialRecord>(agentContext.Wallet, credentialId);
        }

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
                ConnectionId = connection?.Id,
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

        /// <inheritdoc />
        public async Task<CredentialRecord> CreateCredentialAsync(IAgentContext agentContext, CredentialOfferMessage message)
        {
            var service = message.GetDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator);
            var credentialRecordId = await ProcessOfferAsync(agentContext, message, null);

            var (request, record) = await CreateRequestAsync(agentContext, credentialRecordId);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            //request.AddDecorator(provisioning.ToServiceDecorator(), DecoratorNames.ServiceDecorator);
            //request.AddReturnRouting();

            var response = await MessageService.SendAsync(
                wallet: agentContext.Wallet,
                message: request,
                recipientKey: service.RecipientKeys.First(),
                endpointUri: service.ServiceEndpoint,
                routingKeys: service.RoutingKeys.ToArray(),
                senderKey: provisioning.Endpoint.Verkey,
                requestResponse: true);

            if (response is PackedMessageContext responseContext)
            {
                var unpacked = await CryptoUtils.UnpackAsync(agentContext.Wallet, responseContext.Payload);
                var unpackedContext = new UnpackedMessageContext(unpacked.Message, provisioning.Endpoint.Verkey);
                var credentialIssueMessage = unpackedContext.GetMessage<CredentialIssueMessage>();
                var recordId = await ProcessCredentialAsync(agentContext, credentialIssueMessage, null);

                return await RecordService.GetAsync<CredentialRecord>(agentContext.Wallet, recordId);
            }
            throw new ArgumentNullException(nameof(response), "The received response did not contain credential data");
        }

        /// <inheritdoc />
        public async Task<(CredentialRequestMessage, CredentialRecord)> CreateRequestAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credential.State}'");

            string proverDid = null;
            if (credential.ConnectionId != null)
            {
                var connection = await ConnectionService.GetAsync(agentContext, credential.ConnectionId);
                proverDid = connection.MyDid;
            }
            else
            {
                var newDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");
                proverDid = newDid.Did;
            }
            
            var definition = await LedgerService.LookupDefinitionAsync(await agentContext.Pool, credential.CredentialDefinitionId);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            var request = await AnonCreds.ProverCreateCredentialReqAsync(
                wallet: agentContext.Wallet,
                proverDid: proverDid,
                credOfferJson: credential.OfferJson,
                credDefJson: definition.ObjectJson,
                masterSecretId: provisioning.MasterSecretId);

            // Update local credential record with new info
            credential.CredentialRequestMetadataJson = request.CredentialRequestMetadataJson;

            await credential.TriggerAsync(CredentialTrigger.Request);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);

            var threadId = credential.GetTag(TagConstants.LastThreadId);
            var response = new CredentialRequestMessage
            {
                // The comment was required by Aca-py, even though it is declared optional in RFC-0036
                // Was added for interoperability
                Comment = "",
                Requests = new[]
                {
                    new Attachment
                    {
                        Id = "libindy-cred-request-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = request.CredentialRequestJson.GetUTF8Bytes().ToBase64String()
                        }
                    }
                }
            };

            response.ThreadFrom(threadId);
            return (response, credential);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<(CredentialOfferMessage, CredentialRecord)> CreateOfferAsync(
            IAgentContext agentContext, OfferConfiguration config, string connectionId)
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
                    Attributes = credentialRecord.CredentialAttributesValues.Select(x => new CredentialPreviewAttribute
                    {
                        Name = x.Name,
                        MimeType = x.MimeType,
                        Value = x.Value?.ToString()
                    }).ToArray()
                } : null
            }, credentialRecord);
        }

        /// <inheritdoc />
        public async Task<(CredentialOfferMessage, CredentialRecord)> CreateOfferAsync(
            IAgentContext agentContext, OfferConfiguration config)
        {
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (config.CredentialAttributeValues == null || !config.CredentialAttributeValues.Any())
            {
                throw new InvalidOperationException("You must supply credential values when creating connectionless credential offer");
            }

            var (message, record) = await CreateOfferAsync(agentContext, config, null);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            message.AddDecorator(provisioning.ToServiceDecorator(), DecoratorNames.ServiceDecorator);
            record.SetTag("OfferData", message.ToByteArray().ToBase64UrlString());

            await RecordService.UpdateAsync(agentContext.Wallet, record);

            return (message, record);
        }

        /// <inheritdoc />
        public async Task<string> ProcessCredentialRequestAsync(IAgentContext agentContext, CredentialRequestMessage credentialRequest, ConnectionRecord connection)
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
            credential.ConnectionId = connection?.Id;

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

        /// <inheritdoc />
        public Task<(CredentialIssueMessage, CredentialRecord)> CreateCredentialAsync(IAgentContext agentContext, string credentialId)
        {
            return CreateCredentialAsync(agentContext, credentialId, values: null);
        }

        /// <inheritdoc />
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

            if (credential.ConnectionId != null)
            {
                var connection = await ConnectionService.GetAsync(agentContext, credential.ConnectionId);
                if (connection.State != ConnectionState.Connected)
                    throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                        $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");
            }
            
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
                var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);
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
                Credentials = new[]
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