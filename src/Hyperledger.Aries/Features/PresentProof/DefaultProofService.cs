using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Decorators.Service;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof.Messages;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.LedgerApi;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Decorators.Service;
using Hyperledger.Aries.Attachments.Abstractions;
using Hyperledger.Aries.Attachments;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof Service
    /// </summary>
    /// <seealso cref="IProofService" />
    public class DefaultProofService : IProofService
    {
        /// <summary>
        /// The event aggregator
        /// </summary>
        protected readonly IEventAggregator EventAggregator;

        /// <summary>
        /// The connection service
        /// </summary>
        protected readonly IConnectionService ConnectionService;

        /// <summary>
        /// The record service
        /// </summary>
        protected readonly IWalletRecordService RecordService;

        /// <summary>
        /// The provisioning service
        /// </summary>
        protected readonly IProvisioningService ProvisioningService;

        /// <summary>
        /// The ledger service
        /// </summary>
        protected readonly ILedgerService LedgerService;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<DefaultProofService> Logger;

        /// <summary>
        /// The tails service
        /// </summary>
        protected readonly ITailsService TailsService;

        /// <summary>
        /// Message Service
        /// </summary>
        protected readonly IMessageService MessageService;

        /// <summary>
        /// Message Service
        /// </summary>
        private readonly IAttachmentService AttachmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProofService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="connectionService">The connection service.</param>
        /// <param name="recordService">The record service.</param>
        /// <param name="provisioningService">The provisioning service.</param>
        /// <param name="ledgerService">The ledger service.</param>
        /// <param name="tailsService">The tails service.</param>
        /// <param name="messageService">The message service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultProofService(
            IEventAggregator eventAggregator,
            IConnectionService connectionService,
            IWalletRecordService recordService,
            IProvisioningService provisioningService,
            ILedgerService ledgerService,
            ITailsService tailsService,
            IMessageService messageService,
            IAttachmentService attachmentService,
            ILogger<DefaultProofService> logger
        )
        {
            EventAggregator = eventAggregator;
            TailsService = tailsService;
            MessageService = messageService;
            AttachmentService = attachmentService;
            ConnectionService = connectionService;
            RecordService = recordService;
            ProvisioningService = provisioningService;
            LedgerService = ledgerService;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<string> CreateProofAsync(IAgentContext agentContext,
            ProofRequest proofRequest, RequestedCredentials requestedCredentials)
        {
            var provisioningRecord = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            var credentialObjects = new List<CredentialInfo>();
            foreach (var credId in requestedCredentials.GetCredentialIdentifiers())
            {
                var credentialInfo = JsonConvert.DeserializeObject<CredentialInfo>(
                    await AnonCreds.ProverGetCredentialAsync(agentContext.Wallet, credId));

                credentialObjects.Add(credentialInfo);
            }

            var schemas = await BuildSchemasAsync(
                agentContext: agentContext,
                schemaIds: credentialObjects.Select(x => x.SchemaId).Distinct());

            var definitions = await BuildCredentialDefinitionsAsync(
                agentContext: agentContext,
                credentialDefIds: credentialObjects.Select(x => x.CredentialDefinitionId).Distinct());

            var revocationStates = await BuildRevocationStatesAsync(
                agentContext: agentContext,
                credentialObjects: credentialObjects,
                proofRequest: proofRequest,
                requestedCredentials: requestedCredentials);

            var proofJson = await AnonCreds.ProverCreateProofAsync(
                wallet: agentContext.Wallet,
                proofRequest: proofRequest.ToJson(),
                requestedCredentials: requestedCredentials.ToJson(),
                masterSecret: provisioningRecord.MasterSecretId,
                schemas: schemas,
                credentialDefs: definitions,
                revStates: revocationStates);

            return proofJson;
        }

        /// <inheritdoc />
        public virtual async Task<ProofRecord> CreatePresentationAsync(IAgentContext agentContext, RequestPresentationMessage requestPresentation, RequestedCredentials requestedCredentials)
        {
            var service = requestPresentation.GetDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator);

            var record = await ProcessRequestAsync(agentContext, requestPresentation, null);
            var (presentationMessage, proofRecord) = await CreatePresentationAsync(agentContext, record.Id, requestedCredentials);

            await MessageService.SendAsync(
                agentContext: agentContext,
                message: presentationMessage,
                recipientKey: service.RecipientKeys.First(),
                endpointUri: service.ServiceEndpoint,
                routingKeys: service.RoutingKeys?.ToArray());

            return proofRecord;
        }

        /// <inheritdoc />
        public virtual async Task RejectProofRequestAsync(IAgentContext agentContext, string proofRequestId)
        {
            var request = await GetAsync(agentContext, proofRequestId);

            if (request.State != ProofState.Requested)
                throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Proof record state was invalid. Expected '{ProofState.Requested}', found '{request.State}'");

            await request.TriggerAsync(ProofTrigger.Reject);
            await RecordService.UpdateAsync(agentContext.Wallet, request);
        }

        /// <inheritdoc />
        public async Task<bool> IsRevokedAsync(IAgentContext context, string credentialRecordId)
        {
            return await IsRevokedAsync(context, await RecordService.GetAsync<CredentialRecord>(context.Wallet, credentialRecordId));
        }

        /// <inheritdoc />
        public async Task<bool> IsRevokedAsync(IAgentContext context, CredentialRecord record)
        {
            if (record.RevocationRegistryId == null) return false;
            if (record.State == CredentialState.Offered || record.State == CredentialState.Requested) return false;
            if (record.State == CredentialState.Revoked || record.State == CredentialState.Rejected) return true;

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var proofRequest = new ProofRequest
            {
                Name = "revocation check",
                Version = "1.0",
                Nonce = await AnonCreds.GenerateNonceAsync(),
                RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                {
                    { "referent1", new ProofAttributeInfo { Name = record.CredentialAttributesValues.First().Name } }
                }, NonRevoked = new RevocationInterval
                {
                    From = (uint)now,
                    To = (uint)now
                }
            };

            var proof = await CreateProofAsync(context, proofRequest, new RequestedCredentials
            {
                RequestedAttributes = new Dictionary<string, RequestedAttribute>
                {
                    { "referent1", new RequestedAttribute { CredentialId = record.CredentialId, Timestamp = now, Revealed = true } }
                }
            });

            var isValid = await VerifyProofAsync(context, proofRequest.ToJson(), proof);

            if (!isValid)
            {
                await record.TriggerAsync(CredentialTrigger.Revoke);

                record.SetTag("LastRevocationCheck", now.ToString());
                await RecordService.UpdateAsync(context.Wallet, record);
            }

            return !isValid;
        }

        /// <inheritdoc />
        public virtual async Task<bool> VerifyProofAsync(IAgentContext agentContext, string proofRequestJson, string proofJson, bool validateEncoding = true)
        {
            var proof = JsonConvert.DeserializeObject<PartialProof>(proofJson);
            var proofRequest = proofRequestJson.ToObject<ProofRequest>();

            // If any values are revealed, validate encoding
            // against expected values
            if (validateEncoding && proof.RequestedProof.RevealedAttributes != null)
                foreach (var attribute in proof.RequestedProof.RevealedAttributes)
                {
                    if (!CredentialUtils.CheckValidEncoding(attribute.Value.Raw, attribute.Value.Encoded))
                    {
                        throw new AriesFrameworkException(ErrorCode.InvalidProofEncoding, 
                            $"The encoded value for '{attribute.Key}' is invalid. " +
                            $"Expected '{CredentialUtils.GetEncoded(attribute.Value.Raw)}'. " +
                            $"Actual '{attribute.Value.Encoded}'");
                    }
                }

            var schemas = await BuildSchemasAsync(agentContext,
                proof.Identifiers
                    .Select(x => x.SchemaId)
                    .Where(x => x != null)
                    .Distinct());

            var definitions = await BuildCredentialDefinitionsAsync(agentContext,
                proof.Identifiers
                    .Select(x => x.CredentialDefintionId)
                    .Where(x => x != null)
                    .Distinct());

            var revocationDefinitions = await BuildRevocationRegistryDefinitionsAsync(agentContext,
                proof.Identifiers
                    .Select(x => x.RevocationRegistryId)
                    .Where(x => x != null)
                    .Distinct());

            var revocationRegistries = await BuildRevocationRegistriesAsync(
                agentContext,
                proof.Identifiers.Where(x => x.RevocationRegistryId != null));

            return await AnonCreds.VerifierVerifyProofAsync(
                proofRequestJson, 
                proofJson, 
                schemas,
                definitions, 
                revocationDefinitions, 
                revocationRegistries);
        }

        /// <inheritdoc />
        public virtual async Task<bool> VerifyProofAsync(IAgentContext agentContext, string proofRecId)
        {
            var proofRecord = await GetAsync(agentContext, proofRecId);

            if (proofRecord.State != ProofState.Accepted)
                throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Proof record state was invalid. Expected '{ProofState.Accepted}', found '{proofRecord.State}'");

            return await VerifyProofAsync(agentContext, proofRecord.RequestJson, proofRecord.ProofJson);
        }

        /// <inheritdoc />
        public virtual Task<List<ProofRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null,
            int count = 100) => RecordService.SearchAsync<ProofRecord>(agentContext.Wallet, query, null, count);

        /// <inheritdoc />
        public virtual async Task<ProofRecord> GetAsync(IAgentContext agentContext, string proofRecId)
        {
            Logger.LogInformation(LoggingEvents.GetProofRecord, "ProofRecordId {0}", proofRecId);

            return await RecordService.GetAsync<ProofRecord>(agentContext.Wallet, proofRecId) ??
                   throw new AriesFrameworkException(ErrorCode.RecordNotFound, "Proof record not found");
        }

        /// <inheritdoc />
        public virtual async Task<List<Credential>> ListCredentialsForProofRequestAsync(IAgentContext agentContext,
            ProofRequest proofRequest, string attributeReferent)
        {
            using (var search =
                await AnonCreds.ProverSearchCredentialsForProofRequestAsync(agentContext.Wallet, proofRequest.ToJson()))
            {
                var searchResult = await search.NextAsync(attributeReferent, 100);
                return JsonConvert.DeserializeObject<List<Credential>>(searchResult);
            }
        }

        /// <inheritdoc />
        public async Task<PresentationAcknowledgeMessage> CreateAcknowledgeMessageAsync(IAgentContext agentContext, string proofRecordId, string status = AcknowledgementStatusConstants.Ok)
        {
            var record = await GetAsync(agentContext, proofRecordId);
            
            var threadId = record.GetTag(TagConstants.LastThreadId);
            var acknowledgeMessage = new PresentationAcknowledgeMessage(agentContext.UseMessageTypesHttps)
            {
                Id = threadId,
                Status = status
            };
            acknowledgeMessage.ThreadFrom(threadId);

            return acknowledgeMessage;
        }
        
        /// <inheritdoc />
        public virtual async Task<ProofRecord> ProcessAcknowledgeMessageAsync(IAgentContext agentContext, PresentationAcknowledgeMessage acknowledgeMessage)
        {
            var proofRecord = await this.GetByThreadIdAsync(agentContext, acknowledgeMessage.GetThreadId());

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = proofRecord.Id,
                MessageType = acknowledgeMessage.Type,
                ThreadId = acknowledgeMessage.GetThreadId()
            });
            
            return proofRecord;
        }
        
        /// <inheritdoc />
        public virtual async Task<(ProposePresentationMessage, ProofRecord)> CreateProposalAsync(IAgentContext agentContext, ProofProposal proofProposal, string connectionId)
        {
            Logger.LogInformation(LoggingEvents.CreateProofRequest, "ConnectionId {0}", connectionId);

            if (proofProposal == null)
            {
                throw new ArgumentNullException(nameof(proofProposal), "You must provide a presentation preview"); ;
            }
            if (connectionId != null)
            {
                var connection = await ConnectionService.GetAsync(agentContext, connectionId);

                if (connection.State != ConnectionState.Connected)
                    throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                        $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");
            }
            this.CheckProofProposalParameters(proofProposal);


            var threadId = Guid.NewGuid().ToString();
            var proofRecord = new ProofRecord
            {
                Id = Guid.NewGuid().ToString(),
                ConnectionId = connectionId,
                ProposalJson = proofProposal.ToJson(),
                State = ProofState.Proposed
            };

            proofRecord.SetTag(TagConstants.Role, TagConstants.Holder);
            proofRecord.SetTag(TagConstants.LastThreadId, threadId);

            await RecordService.AddAsync(agentContext.Wallet, proofRecord);

            var message = new ProposePresentationMessage(agentContext.UseMessageTypesHttps)
            {
                Id = threadId,
                Comment = proofProposal.Comment,
                PresentationPreviewMessage = new PresentationPreviewMessage(agentContext.UseMessageTypesHttps)
                {
                    ProposedAttributes = proofProposal.ProposedAttributes.ToArray(),
                    ProposedPredicates = proofProposal.ProposedPredicates.ToArray()
                },
            };
            message.ThreadFrom(threadId);
            return (message, proofRecord);
        }

        public virtual async Task<ProofRecord> ProcessProposalAsync(IAgentContext agentContext, ProposePresentationMessage proposePresentationMessage, ConnectionRecord connection)
        {
            // save in wallet

            var proofProposal = new ProofProposal
            {
                Comment = proposePresentationMessage.Comment,
                ProposedAttributes = proposePresentationMessage.PresentationPreviewMessage.ProposedAttributes.ToList<ProposedAttribute>(),
                ProposedPredicates = proposePresentationMessage.PresentationPreviewMessage.ProposedPredicates.ToList<ProposedPredicate>()
            };

            var proofRecord = new ProofRecord
            {
                Id = Guid.NewGuid().ToString(),
                ProposalJson = proofProposal.ToJson(),
                ConnectionId = connection?.Id,
                State = ProofState.Proposed
            };

            proofRecord.SetTag(TagConstants.LastThreadId, proposePresentationMessage.GetThreadId());
            proofRecord.SetTag(TagConstants.Role, TagConstants.Requestor);
            await RecordService.AddAsync(agentContext.Wallet, proofRecord);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = proofRecord.Id,
                MessageType = proposePresentationMessage.Type,
                ThreadId = proposePresentationMessage.GetThreadId()
            });

            return proofRecord;
        }

        /// <inheritdoc />
        public async Task<(RequestPresentationMessage, ProofRecord)> CreateRequestFromProposalAsync(IAgentContext agentContext, ProofRequestParameters requestParams,
            string proofRecordId, string connectionId)
        {
            Logger.LogInformation(LoggingEvents.CreateProofRequest, "ConnectionId {0}", connectionId);

            if (proofRecordId == null)
            {
                throw new ArgumentNullException(nameof(proofRecordId), "You must provide proof record Id");
            }
            if (connectionId != null)
            {
                var connection = await ConnectionService.GetAsync(agentContext, connectionId);

                if (connection.State != ConnectionState.Connected)
                    throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                        $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");
            }

            var proofRecord = await RecordService.GetAsync<ProofRecord>(agentContext.Wallet, proofRecordId);
            var proofProposal = proofRecord.ProposalJson.ToObject<ProofProposal>();


            // Build Proof Request from Proposal info
            var proofRequest = new ProofRequest
            {
                Name = requestParams.Name,
                Version = requestParams.Version,
                Nonce = await AnonCreds.GenerateNonceAsync(),
                RequestedAttributes = new Dictionary<string, ProofAttributeInfo>(),
                NonRevoked = requestParams.NonRevoked
            };

            var attributesByReferent = new Dictionary<string, List<ProposedAttribute>>();
            foreach (var proposedAttribute in proofProposal.ProposedAttributes)
            {
                if (proposedAttribute.Referent == null)
                {
                    proposedAttribute.Referent = Guid.NewGuid().ToString();
                }

                if (attributesByReferent.TryGetValue(proposedAttribute.Referent, out var referentAttributes))
                {
                    referentAttributes.Add(proposedAttribute);
                }
                else
                {
                    attributesByReferent.Add(proposedAttribute.Referent, new List<ProposedAttribute> { proposedAttribute });
                }
            }

            foreach (var referent in attributesByReferent.AsEnumerable())
            {
                var proposedAttributes = referent.Value;
                var attributeName = proposedAttributes.Count() == 1 ? proposedAttributes.Single().Name : null;
                var attributeNames = proposedAttributes.Count() > 1 ? proposedAttributes.ConvertAll<string>(r => r.Name).ToArray() : null;


                var requestedAttribute = new ProofAttributeInfo()
                {
                    Name = attributeName,
                    Names = attributeNames,
                    Restrictions = new List<AttributeFilter>
                    {
                        new AttributeFilter {
                            CredentialDefinitionId = proposedAttributes.First().CredentialDefinitionId,
                            SchemaId = proposedAttributes.First().SchemaId,
                            IssuerDid = proposedAttributes.First().IssuerDid
                        }
                    }
                };
                proofRequest.RequestedAttributes.Add(referent.Key, requestedAttribute);
                Console.WriteLine($"Added Attribute to Proof Request \n {proofRequest.ToString()}");
            }

            foreach (var pred in proofProposal.ProposedPredicates)
            {
                if (pred.Referent == null)
                {
                    pred.Referent = Guid.NewGuid().ToString();
                }
                var predicate = new ProofPredicateInfo()
                {
                    Name = pred.Name,
                    PredicateType = pred.Predicate,
                    PredicateValue = pred.Threshold,
                    Restrictions = new List<AttributeFilter>
                    {
                        new AttributeFilter {
                            CredentialDefinitionId = pred.CredentialDefinitionId,
                            SchemaId = pred.SchemaId,
                            IssuerDid = pred.IssuerDid
                        }
                    }

                };
                proofRequest.RequestedPredicates.Add(pred.Referent, predicate);
            }

            proofRecord.RequestJson = proofRequest.ToJson();
            await proofRecord.TriggerAsync(ProofTrigger.Request);
            await RecordService.UpdateAsync(agentContext.Wallet, proofRecord);

            var message = new RequestPresentationMessage(agentContext.UseMessageTypesHttps)
            {
                Id = proofRecord.Id,
                Requests = new[]
                {
                    new Attachment
                    {
                        Id = "libindy-request-presentation-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = proofRequest
                                .ToJson()
                                .GetUTF8Bytes()
                                .ToBase64String()
                        }
                    }
                }
            };
            message.ThreadFrom(proofRecord.GetTag(TagConstants.LastThreadId));
            return (message, proofRecord);
        }

        /// <inheritdoc />
        public Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(
            IAgentContext agentContext,
            ProofRequest proofRequest,
            string connectionId) =>
            CreateRequestAsync(
                agentContext: agentContext,
                proofRequestJson: proofRequest?.ToJson(),
                connectionId: connectionId);

        /// <inheritdoc />
        public virtual async Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext, string proofRequestJson, string connectionId)
        {
            Logger.LogInformation(LoggingEvents.CreateProofRequest, "ConnectionId {0}", connectionId);

            if (proofRequestJson == null)
            {
                throw new ArgumentNullException(nameof(proofRequestJson), "You must provide proof request");
            }
            if (connectionId != null)
            {
                var connection = await ConnectionService.GetAsync(agentContext, connectionId);

                if (connection.State != ConnectionState.Connected)
                    throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                        $"Connection state was invalid. Expected '{ConnectionState.Connected}', found '{connection.State}'");
            }

            var threadId = Guid.NewGuid().ToString();
            var proofRecord = new ProofRecord
            {
                Id = Guid.NewGuid().ToString(),
                ConnectionId = connectionId,
                RequestJson = proofRequestJson
            };
            proofRecord.SetTag(TagConstants.Role, TagConstants.Requestor);
            proofRecord.SetTag(TagConstants.LastThreadId, threadId);
            await RecordService.AddAsync(agentContext.Wallet, proofRecord);

            var message = new RequestPresentationMessage(agentContext.UseMessageTypesHttps)
            {
                Id = threadId,
                Requests = new[]
                {
                    new Attachment
                    {
                        Id = "libindy-request-presentation-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = proofRequestJson
                                .GetUTF8Bytes()
                                .ToBase64String()
                        }
                    }
                }
            };
            message.ThreadFrom(threadId);
            return (message, proofRecord);
        }
        
        /// <inheritdoc />
        public virtual async Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext, ProofRequest proofRequest, bool useDidKeyFormat = false)
        {
            var (message, record) = await CreateRequestAsync(agentContext, proofRequest, null);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            message.AddDecorator(provisioning.ToServiceDecorator(useDidKeyFormat), DecoratorNames.ServiceDecorator);
            record.SetTag("RequestData", message.ToByteArray().ToBase64UrlString());

            return (message, record);
        }

        /// <inheritdoc />
        public virtual async Task<ProofRecord> ProcessRequestAsync(IAgentContext agentContext, RequestPresentationMessage requestPresentationMessage, ConnectionRecord connection)
        {
            var requestAttachment = requestPresentationMessage.Requests.FirstOrDefault(x => x.Id == "libindy-request-presentation-0")
                ?? throw new ArgumentException("Presentation request attachment not found.");

            var requestJson = requestAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();

            ProofRecord proofRecord = null;

            try
            {
                proofRecord = await this.GetByThreadIdAsync(agentContext, requestPresentationMessage.GetThreadId());
            }
            catch (AriesFrameworkException e)
            {
                if (e.ErrorCode != ErrorCode.RecordNotFound)
                {
                    throw;
                }
            }

            if (proofRecord is null)
            {
                proofRecord = new ProofRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    RequestJson = requestJson,
                    ConnectionId = connection?.Id,
                    State = ProofState.Requested
                };
                proofRecord.SetTag(TagConstants.LastThreadId, requestPresentationMessage.GetThreadId());
                proofRecord.SetTag(TagConstants.Role, TagConstants.Holder);
                if (requestPresentationMessage.Comment != null)
                    proofRecord.SetTag(TagConstants.Comment, requestPresentationMessage.Comment);
                await RecordService.AddAsync(agentContext.Wallet, proofRecord);
            }
            else
            {
                await proofRecord.TriggerAsync(ProofTrigger.Request);
                proofRecord.RequestJson = requestJson;
                await RecordService.UpdateAsync(agentContext.Wallet, proofRecord);
            }
            
            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = proofRecord.Id,
                MessageType = requestPresentationMessage.Type,
                ThreadId = requestPresentationMessage.GetThreadId()
            });

            return proofRecord;
        }

        /// <inheritdoc />
        public virtual async Task<ProofRecord> ProcessPresentationAsync(IAgentContext agentContext, PresentationMessage presentationMessage)
        {
            var proofRecord = await this.GetByThreadIdAsync(agentContext, presentationMessage.GetThreadId());

            var requestAttachment = presentationMessage.Presentations.FirstOrDefault(x => x.Id == "libindy-presentation-0")
                ?? throw new ArgumentException("Presentation attachment not found.");

            var attachment = presentationMessage.GetAttachment(Nicknames.File);
            if (attachment != null)
                await AttachmentService.Create(agentContext, presentationMessage, proofRecord.Id, Nicknames.File);

            var proofJson = requestAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();

            if (proofRecord.State != ProofState.Requested)
                throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Proof state was invalid. Expected '{ProofState.Requested}', found '{proofRecord.State}'");

            proofRecord.ProofJson = proofJson;
            await proofRecord.TriggerAsync(ProofTrigger.Accept);
            await RecordService.UpdateAsync(agentContext.Wallet, proofRecord);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = proofRecord.Id,
                MessageType = presentationMessage.Type,
                ThreadId = presentationMessage.GetThreadId()
            });

            return proofRecord;
        }

        /// <inheritdoc />
        public virtual Task<string> CreatePresentationAsync(IAgentContext agentContext, ProofRequest proofRequest, RequestedCredentials requestedCredentials) =>
            CreateProofAsync(agentContext, proofRequest, requestedCredentials);

        /// <inheritdoc />
        public virtual async Task<(PresentationMessage, ProofRecord)> CreatePresentationAsync(IAgentContext agentContext, string proofRecordId, RequestedCredentials requestedCredentials)
        {
            var record = await GetAsync(agentContext, proofRecordId);

            if (record.State != ProofState.Requested)
                throw new AriesFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Proof state was invalid. Expected '{ProofState.Requested}', found '{record.State}'");
            var proofJson = await CreatePresentationAsync(
                agentContext,
                record.RequestJson.ToObject<ProofRequest>(),
                requestedCredentials);

            record.ProofJson = proofJson;
            await record.TriggerAsync(ProofTrigger.Accept);
            await RecordService.UpdateAsync(agentContext.Wallet, record);

            var threadId = record.GetTag(TagConstants.LastThreadId);

            var proofMsg = new PresentationMessage(agentContext.UseMessageTypesHttps)
            {
                Id = Guid.NewGuid().ToString(),
                Presentations = new[]
                {
                    new Attachment
                    {
                        Id = "libindy-presentation-0",
                        MimeType = CredentialMimeTypes.ApplicationJsonMimeType,
                        Data = new AttachmentContent
                        {
                            Base64 = proofJson
                                .GetUTF8Bytes()
                                .ToBase64String()
                        }
                    }
                }
            };
            proofMsg.ThreadFrom(threadId);

            return (proofMsg, record);
        }

        #region Private Methods

        private async Task<string> BuildSchemasAsync(IAgentContext agentContext, IEnumerable<string> schemaIds)
        {
            var result = new Dictionary<string, JObject>();

            foreach (var schemaId in schemaIds)
            {
                var ledgerSchema = await LedgerService.LookupSchemaAsync(agentContext, schemaId);
                result.Add(schemaId, JObject.Parse(ledgerSchema.ObjectJson));
            }

            return result.ToJson();
        }

        private async Task<string> BuildCredentialDefinitionsAsync(IAgentContext agentContext, IEnumerable<string> credentialDefIds)
        {
            var result = new Dictionary<string, JObject>();

            foreach (var schemaId in credentialDefIds)
            {
                var ledgerDefinition = await LedgerService.LookupDefinitionAsync(agentContext, schemaId);
                result.Add(schemaId, JObject.Parse(ledgerDefinition.ObjectJson));
            }

            return result.ToJson();
        }

        private bool HasNonRevokedOnAttributeLevel(ProofRequest proofRequest)
        {
            foreach (var proofRequestRequestedAttribute in proofRequest.RequestedAttributes)
                if (proofRequestRequestedAttribute.Value.NonRevoked != null)
                    return true;

            foreach (var proofRequestRequestedPredicate in proofRequest.RequestedPredicates)
                if (proofRequestRequestedPredicate.Value.NonRevoked != null)
                    return true;

            return false;
        }

        private async Task<(ParseRegistryResponseResult, string)> BuildRevocationStateAsync(
            IAgentContext agentContext, CredentialInfo credential, ParseResponseResult registryDefinition,
            RevocationInterval nonRevoked)
        {
            var delta = await LedgerService.LookupRevocationRegistryDeltaAsync(
                agentContext: agentContext,
                revocationRegistryId: credential.RevocationRegistryId,
                // Ledger will not return correct revocation state if the 'from' field
                // is other than 0
                from: 0, //nonRevoked.From,
                to: nonRevoked.To);

            var tailsFile = await TailsService.EnsureTailsExistsAsync(agentContext, credential.RevocationRegistryId);
            var tailsReader = await TailsService.OpenTailsAsync(tailsFile);

            var state = await AnonCreds.CreateRevocationStateAsync(
                blobStorageReader: tailsReader,
                revRegDef: registryDefinition.ObjectJson,
                revRegDelta: delta.ObjectJson,
                timestamp: (long)delta.Timestamp,
                credRevId: credential.CredentialRevocationId);

            return (delta, state);
        }

        private async Task<string> BuildRevocationStatesAsync(IAgentContext agentContext,
            IEnumerable<CredentialInfo> credentialObjects,
            ProofRequest proofRequest,
            RequestedCredentials requestedCredentials)
        {
            var allCredentials = new List<RequestedAttribute>();
            allCredentials.AddRange(requestedCredentials.RequestedAttributes.Values);
            allCredentials.AddRange(requestedCredentials.RequestedPredicates.Values);

            var result = new Dictionary<string, Dictionary<string, JObject>>();
            
            if (proofRequest.NonRevoked == null && !HasNonRevokedOnAttributeLevel(proofRequest))
                return result.ToJson();

            foreach (var requestedCredential in allCredentials)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var credential = credentialObjects.First(x => x.Referent == requestedCredential.CredentialId);
                if (credential.RevocationRegistryId == null)
                    continue;

                var registryDefinition = await LedgerService.LookupRevocationRegistryDefinitionAsync(
                    agentContext: agentContext,
                    registryId: credential.RevocationRegistryId);

                if (proofRequest.NonRevoked != null)
                {
                    var (delta, state) = await BuildRevocationStateAsync(
                        agentContext, credential, registryDefinition, proofRequest.NonRevoked);
                    
                    if (!result.ContainsKey(credential.RevocationRegistryId))
                        result.Add(credential.RevocationRegistryId, new Dictionary<string, JObject>());

                    requestedCredential.Timestamp = (long) delta.Timestamp;
                    if (!result[credential.RevocationRegistryId].ContainsKey($"{delta.Timestamp}"))
                        result[credential.RevocationRegistryId].Add($"{delta.Timestamp}", JObject.Parse(state));
                    
                    continue;
                }

                foreach (var proofRequestRequestedAttribute in proofRequest.RequestedAttributes)
                {
                    var revocationInterval = proofRequestRequestedAttribute.Value.NonRevoked;
                    var (delta, state) = await BuildRevocationStateAsync(
                        agentContext, credential, registryDefinition, revocationInterval);
                    
                    if (!result.ContainsKey(credential.RevocationRegistryId))
                        result.Add(credential.RevocationRegistryId, new Dictionary<string, JObject>());

                    requestedCredential.Timestamp = (long) delta.Timestamp;
                    if (!result[credential.RevocationRegistryId].ContainsKey($"{delta.Timestamp}"))
                        result[credential.RevocationRegistryId].Add($"{delta.Timestamp}", JObject.Parse(state));
                }

                foreach (var proofRequestRequestedPredicate in proofRequest.RequestedPredicates)
                {
                    var revocationInterval = proofRequestRequestedPredicate.Value.NonRevoked;
                    var (delta, state) = await BuildRevocationStateAsync(
                        agentContext, credential, registryDefinition, revocationInterval);
                    
                    if (!result.ContainsKey(credential.RevocationRegistryId))
                        result.Add(credential.RevocationRegistryId, new Dictionary<string, JObject>());

                    requestedCredential.Timestamp = (long) delta.Timestamp;
                    if (!result[credential.RevocationRegistryId].ContainsKey($"{delta.Timestamp}"))
                        result[credential.RevocationRegistryId].Add($"{delta.Timestamp}", JObject.Parse(state));
                }
            }

            return result.ToJson();
        }

        private async Task<string> BuildRevocationRegistriesAsync(
            IAgentContext agentContext,
            IEnumerable<ProofIdentifier> proofIdentifiers)
        {
            var result = new Dictionary<string, Dictionary<string, JObject>>();

            foreach (var identifier in proofIdentifiers)
            {
                if (identifier.Timestamp == null) continue;

                var revocationRegistry = await LedgerService.LookupRevocationRegistryAsync(
                    agentContext,
                    identifier.RevocationRegistryId,
                    long.Parse(identifier.Timestamp));

                result.Add(identifier.RevocationRegistryId,
                    new Dictionary<string, JObject>
                    {
                        {identifier.Timestamp, JObject.Parse(revocationRegistry.ObjectJson)}
                    });
            }

            return result.ToJson();
        }

        private async Task<string> BuildRevocationRegistryDefinitionsAsync(IAgentContext agentContext,
            IEnumerable<string> revocationRegistryIds)
        {
            var result = new Dictionary<string, JObject>();

            foreach (var revocationRegistryId in revocationRegistryIds)
            {
                var ledgerSchema =
                    await LedgerService.LookupRevocationRegistryDefinitionAsync(agentContext, revocationRegistryId);
                result.Add(revocationRegistryId, JObject.Parse(ledgerSchema.ObjectJson));
            }

            return result.ToJson();
        }

        private void CheckProofProposalParameters(ProofProposal proofProposal)
        {
            if (proofProposal.ProposedAttributes.Count > 1)
            {
                var attrList = proofProposal.ProposedAttributes;
                var referents = new Dictionary<string, ProposedAttribute>();

                // Check if all attributes that share referent have same requirements
                for (int i = 0; i < attrList.Count; i++)
                {
                    var attr = attrList[i];
                    if (referents.ContainsKey(attr.Referent))
                    {
                        if(referents[attr.Referent].IssuerDid != attr.IssuerDid ||
                           referents[attr.Referent].SchemaId != attr.SchemaId ||
                           referents[attr.Referent].CredentialDefinitionId != attr.CredentialDefinitionId)
                        {
                            throw new AriesFrameworkException(ErrorCode.InvalidParameterFormat, "All attributes that share a referent must have identical requirements");
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        referents.Add(attr.Referent, attr);
                    }
                }
            }
            
            if (proofProposal.ProposedPredicates.Count > 1)
            {
                var predList = proofProposal.ProposedPredicates;
                var referents = new Dictionary<string, ProposedPredicate>();

                for (int i = 0; i < predList.Count; i++)
                {
                    var pred = predList[i];
                    if (referents.ContainsKey(pred.Referent))
                    {
                        throw new AriesFrameworkException(ErrorCode.InvalidParameterFormat, "Proposed Predicates must all have unique referents");
                    }
                    else
                    {
                        referents.Add(pred.Referent, pred);
                    }
                }
            }
        }

        #endregion
    }
}
