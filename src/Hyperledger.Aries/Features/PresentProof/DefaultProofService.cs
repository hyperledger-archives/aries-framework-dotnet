using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.PoolApi;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Decorators.Service;
using System.Diagnostics;

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
            ILogger<DefaultProofService> logger)
        {
            EventAggregator = eventAggregator;
            TailsService = tailsService;
            MessageService = messageService;
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
        public async Task<ProofRecord> CreatePresentationAsync(IAgentContext agentContext, RequestPresentationMessage requestPresentation, RequestedCredentials requestedCredentials)
        {
            var service = requestPresentation.GetDecorator<ServiceDecorator>(DecoratorNames.ServiceDecorator);

            var record = await ProcessRequestAsync(agentContext, requestPresentation, null);
            var (presentationMessage, proofRecord) = await CreatePresentationAsync(agentContext, record.Id, requestedCredentials);

            await MessageService.SendAsync(
                wallet: agentContext.Wallet,
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
                var result = JsonConvert.DeserializeObject<List<Credential>>(searchResult);

                if (proofRequest.NonRevoked != null)
                {
                    return result.Where(x => x.CredentialInfo.RevocationRegistryId != null).ToList();
                }
                return result;
            }
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

        private async Task<string> BuildRevocationStatesAsync(IAgentContext agentContext,
            IEnumerable<CredentialInfo> credentialObjects,
            ProofRequest proofRequest,
            RequestedCredentials requestedCredentials)
        {
            var allCredentials = new List<RequestedAttribute>();
            allCredentials.AddRange(requestedCredentials.RequestedAttributes.Values);
            allCredentials.AddRange(requestedCredentials.RequestedPredicates.Values);

            var result = new Dictionary<string, Dictionary<string, JObject>>();
            foreach (var requestedCredential in allCredentials)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                var credential = credentialObjects.First(x => x.Referent == requestedCredential.CredentialId);
                if (credential.RevocationRegistryId == null ||
                    (proofRequest.NonRevoked == null))
                    continue;

                var registryDefinition = await LedgerService.LookupRevocationRegistryDefinitionAsync(
                    agentContext: agentContext,
                    registryId: credential.RevocationRegistryId);

                var delta = await LedgerService.LookupRevocationRegistryDeltaAsync(
                    agentContext: agentContext,
                    revocationRegistryId: credential.RevocationRegistryId,
                    from: proofRequest.NonRevoked.From,
                    to: proofRequest.NonRevoked.To);

                var tailsfile = await TailsService.EnsureTailsExistsAsync(agentContext, credential.RevocationRegistryId);
                var tailsReader = await TailsService.OpenTailsAsync(tailsfile);

                var state = await AnonCreds.CreateRevocationStateAsync(
                    blobStorageReader: tailsReader,
                    revRegDef: registryDefinition.ObjectJson,
                    revRegDelta: delta.ObjectJson,
                    timestamp: (long)delta.Timestamp,
                    credRevId: credential.CredentialRevocationId);

                if (!result.ContainsKey(credential.RevocationRegistryId))
                    result.Add(credential.RevocationRegistryId, new Dictionary<string, JObject>());

                requestedCredential.Timestamp = (long)delta.Timestamp;
                if (!result[credential.RevocationRegistryId].ContainsKey($"{delta.Timestamp}"))
                {
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
        public async Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext, string proofRequestJson, string connectionId)
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

            var message = new RequestPresentationMessage
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
        public async Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext, ProofRequest proofRequest)
        {
            var (message, record) = await CreateRequestAsync(agentContext, proofRequest, null);
            var provisioning = await ProvisioningService.GetProvisioningAsync(agentContext.Wallet);

            message.AddDecorator(provisioning.ToServiceDecorator(), DecoratorNames.ServiceDecorator);
            record.SetTag("RequestData", message.ToByteArray().ToBase64UrlString());

            return (message, record);
        }

        /// <inheritdoc />
        public async Task<ProofRecord> ProcessRequestAsync(IAgentContext agentContext, RequestPresentationMessage requestPresentationMessage, ConnectionRecord connection)
        {
            var requestAttachment = requestPresentationMessage.Requests.FirstOrDefault(x => x.Id == "libindy-request-presentation-0")
                ?? throw new ArgumentException("Presentation request attachment not found.");

            var requestJson = requestAttachment.Data.Base64.GetBytesFromBase64().GetUTF8String();

            // Write offer record to local wallet
            var proofRecord = new ProofRecord
            {
                Id = Guid.NewGuid().ToString(),
                RequestJson = requestJson,
                ConnectionId = connection?.Id,
                State = ProofState.Requested
            };
            proofRecord.SetTag(TagConstants.LastThreadId, requestPresentationMessage.GetThreadId());
            proofRecord.SetTag(TagConstants.Role, TagConstants.Holder);

            await RecordService.AddAsync(agentContext.Wallet, proofRecord);

            EventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = proofRecord.Id,
                MessageType = requestPresentationMessage.Type,
                ThreadId = requestPresentationMessage.GetThreadId()
            });

            return proofRecord;
        }

        /// <inheritdoc />
        public async Task<ProofRecord> ProcessPresentationAsync(IAgentContext agentContext, PresentationMessage presentationMessage)
        {
            var proofRecord = await this.GetByThreadIdAsync(agentContext, presentationMessage.GetThreadId());

            var requestAttachment = presentationMessage.Presentations.FirstOrDefault(x => x.Id == "libindy-presentation-0")
                ?? throw new ArgumentException("Presentation attachment not found.");

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
        public Task<string> CreatePresentationAsync(IAgentContext agentContext, ProofRequest proofRequest, RequestedCredentials requestedCredentials) =>
            CreateProofAsync(agentContext, proofRequest, requestedCredentials);

        /// <inheritdoc />
        public async Task<(PresentationMessage, ProofRecord)> CreatePresentationAsync(IAgentContext agentContext, string proofRecordId, RequestedCredentials requestedCredentials)
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

            var proofMsg = new PresentationMessage
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

        public Task<(ProposePresentationMessage, ProofRecord)> CreateProposalAsync(IAgentContext agentContext, AttributePreview[] attributes, PredicatePreviews[] predicates, string connectionId)
        {
            throw new NotImplementedException();
        }

        public Task<(RequestPresentationMessage, ProofRecord)> CreateProposalAsync(IAgentContext agentContext, string proofProposalJson, string connectionId)
        {
            throw new NotImplementedException();
        }

        public Task<ProofRecord> ProcessProposalAsync(IAgentContext agentContext, ProposePresentationMessage proofProposal, ConnectionRecord connection)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}