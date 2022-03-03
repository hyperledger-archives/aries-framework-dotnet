using System;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Common.Dids;
using Hyperledger.Aries.Features.Handshakes.DidExchange.Models;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.DidApi;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange
{
    /// <inheritdoc/>
    public class DefaultDidExchangeService : IDidExchangeService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILedgerService _ledgerService;
        private readonly IProvisioningService _provisioningService;
        private readonly IWalletRecordService _recordService;
        
        /// <summary>Initializes a new instance of the <see cref="DefaultDidExchangeService"/> class.</summary>
        public DefaultDidExchangeService(ILedgerService ledgerService, IWalletRecordService recordService, IProvisioningService provisioningService, IEventAggregator eventAggregator)
        {
            _ledgerService = ledgerService;
            _recordService = recordService;
            _provisioningService = provisioningService;
            _eventAggregator = eventAggregator;
        }

        /// <inheritdoc/>
        public Task<(DidExchangeRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, AgentMessage message)
        {
            // Todo: Create request based on out-of-band message
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<(DidExchangeRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, string did)
        {
            var key = await Did.KeyForDidAsync(await agentContext.Pool, agentContext.Wallet, did);
            var endpointResult = await _ledgerService.LookupServiceEndpointAsync(agentContext, did);
            
            var myDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");

            var connection = new ConnectionRecord
            {
                Endpoint = new AgentEndpoint {Uri = endpointResult.Result.Endpoint},
                MyDid = DidUtils.ConvertVerkeyToDidKey(myDid.VerKey),
                MyVk = myDid.VerKey,
                TheirDid = did,
                TheirVk = key,
                State = ConnectionState.Negotiating,
            };
            await _recordService.AddAsync(agentContext.Wallet, connection);

            var provisioningRecord = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);

            var didDoc = new AttachmentContent
                {Base64 = connection.MyDidDoc(provisioningRecord).ToJson().ToBase64Url()};
            await didDoc.SignWithJsonWebSignature(agentContext.Wallet, myDid.VerKey);
            
            var attachment = new Attachment
            {
                Id = Guid.NewGuid().ToString(),
                MimeType = "application/json",
                Data = didDoc
            };
            
            var request = new DidExchangeRequestMessage
            {
                Did = connection.MyDid,
                Label = provisioningRecord.Owner.Name,
                DidDoc = attachment
            };

            return (request, connection);
        }

        /// <inheritdoc/>
        public async Task<ConnectionRecord> ProcessRequestAsync(IAgentContext agentContext, DidExchangeRequestMessage requestMessage)
        {
            var myDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");

            DidDoc didDoc = null;
            if (requestMessage.DidDoc.Data.Base64 is { } data)
            {
                var isValidSignature = await requestMessage.DidDoc.Data.VerifyJsonWebSignature();
                if (isValidSignature == false)
                    throw new AriesFrameworkException(ErrorCode.InvalidSignatureEncoding,
                        "The given JSON web signature is invalid");
                
                var json = data.FromBase64Url();
                didDoc = json.ToObject<DidDoc>();
            }

            // Todo: Handle resolvable Dids
            if (didDoc == null)
                throw new NotImplementedException("Request message must provide an attached did document");

            if (didDoc.Keys.All(key => key.Type == DidDocExtensions.DefaultKeyType) == false)
            {
                throw new NotImplementedException($"Only {DidDocExtensions.DefaultKeyType} is supported");
            }

            var indyService = (IndyAgentDidDocService)didDoc.Services.First(service => service is IndyAgentDidDocService);

            var agentEndpoint = new AgentEndpoint(indyService.ServiceEndpoint, null, indyService.RoutingKeys.ToArray());
            
            var connectionRecord = new ConnectionRecord
            {
                Id = Guid.NewGuid().ToString(),
                Alias = new ConnectionAlias {Name = requestMessage.Label},
                MyDid = DidUtils.ConvertVerkeyToDidKey(myDid.VerKey),
                MyVk = myDid.VerKey,
                TheirDid = requestMessage.Did,
                TheirVk = didDoc.Keys.FirstOrDefault(key => key.Controller == requestMessage.Did)?.PublicKeyBase58 
                          ?? throw new NullReferenceException("Missing public for controller"),
                Endpoint = agentEndpoint,
                State = ConnectionState.Negotiating
            };
            await _recordService.AddAsync(agentContext.Wallet, connectionRecord);

            _eventAggregator.Publish(
                new ServiceMessageProcessingEvent
                {
                    MessageType = requestMessage.Type,
                    RecordId = connectionRecord.Id,
                    ThreadId = requestMessage.GetThreadId()
                });

            return connectionRecord;
        }

        /// <inheritdoc/>
        public async Task<(DidExchangeResponseMessage, ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Response);
            
            var myDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");

            connectionRecord.MyDid = DidUtils.ConvertVerkeyToDidKey(myDid.VerKey);
            connectionRecord.MyVk = myDid.VerKey;

            var provisioningRecord = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
            var didDoc = new AttachmentContent
                {Base64 = connectionRecord.MyDidDoc(provisioningRecord).ToJson().ToBase64Url()};
            await didDoc.SignWithJsonWebSignature(agentContext.Wallet, myDid.VerKey);
            
            var attachment = new Attachment
            {
                Id = Guid.NewGuid().ToString(),
                MimeType = "application/json",
                Data = didDoc
            };

            var response = new DidExchangeResponseMessage
            {
                Id = Guid.NewGuid().ToString(),
                Did = connectionRecord.MyDid,
                DidDoc = attachment
            };
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);

            return (response, connectionRecord);
        }

        /// <inheritdoc/>
        public async Task<ConnectionRecord> ProcessResponseAsync(IAgentContext agentContext, DidExchangeResponseMessage responseMessage, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Response);
            
            DidDoc didDoc = null;
            if (responseMessage.DidDoc.Data.Base64 is { } data)
            {
                var isValidSignature = await responseMessage.DidDoc.Data.VerifyJsonWebSignature();
                if (isValidSignature == false)
                    throw new AriesFrameworkException(ErrorCode.InvalidSignatureEncoding,
                        "The given JSON web signature is invalid");
                
                var json = data.FromBase64Url();
                didDoc = json.ToObject<DidDoc>();
            }

            if (didDoc == null)
                throw new NotImplementedException("Response message must provide an attached did document");

            if (didDoc.Keys.All(key => key.Type == DidDocExtensions.DefaultKeyType) == false)
            {
                throw new NotImplementedException($"Only {DidDocExtensions.DefaultKeyType} is supported");
            }

            var indyService = (IndyAgentDidDocService)didDoc.Services.First(service => service is IndyAgentDidDocService);

            var agentEndpoint = new AgentEndpoint(indyService.ServiceEndpoint, null, indyService.RoutingKeys.ToArray());

            connectionRecord.TheirDid = responseMessage.Did;
            connectionRecord.TheirVk =
                didDoc.Keys.FirstOrDefault(key => key.Controller == responseMessage.Did)?.PublicKeyBase58
                ?? throw new NullReferenceException("Missing public key for controller");
            connectionRecord.Endpoint = agentEndpoint;

            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);
            
            _eventAggregator.Publish(new ServiceMessageProcessingEvent()
            {
                MessageType = responseMessage.Type,
                RecordId = connectionRecord.Id,
                ThreadId = responseMessage.GetThreadId()
            });

            return connectionRecord;
        }

        /// <inheritdoc/>
        public async Task<(DidExchangeCompleteMessage, ConnectionRecord)> CreateComplete(IAgentContext agentContext, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Complete);
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);

            var completeMessage = new DidExchangeCompleteMessage
            {
                Id = Guid.NewGuid().ToString()
            };

            return (completeMessage, connectionRecord);
        }

        /// <inheritdoc/>
        public async Task<ConnectionRecord> ProcessComplete(IAgentContext agentContext, DidExchangeCompleteMessage completeMessage,
            ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Complete);
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);
            
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                MessageType = completeMessage.Type,
                RecordId = connectionRecord.Id,
                ThreadId = completeMessage.GetThreadId()
            });

            return connectionRecord;
        }

        /// <inheritdoc/>
        public async Task<(DidExchangeProblemReportMessage, ConnectionRecord)> AbandonDidExchange(IAgentContext agentContext, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Abandon);
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);
            
            var myRole = connectionRecord.Role;
            var problemCode = myRole == ConnectionRole.Invitee
                ? DidExchangeProblemReportMessage.Error.ResponseNotAccepted
                : DidExchangeProblemReportMessage.Error.RequestNotAccepted;
            
            var problemReport = new DidExchangeProblemReportMessage {ProblemCode = problemCode};

            return (problemReport, connectionRecord);
        }

        /// <inheritdoc/>
        public async Task<ConnectionRecord> ProcessProblemReportMessage(IAgentContext agentContext, DidExchangeProblemReportMessage problemReportMessage, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Abandon);
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);

            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                MessageType = problemReportMessage.Type,
                RecordId = connectionRecord.Id,
                ThreadId = problemReportMessage.GetThreadId()
            });
            
            return connectionRecord;
        }
    }
}
