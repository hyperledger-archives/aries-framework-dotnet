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
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.PoolApi;

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
        public virtual async Task<ConnectionRecord> ProcessInvitationAsync(IAgentContext agentContext, InvitationMessage invitation)
        {
            if (invitation.HandshakeProtocols.Contains(HandshakeProtocolUri.DidExchange) == false)
                throw new NotImplementedException("The given handshake protocols are not implemented.");

            ConnectionRecord connectionRecord = null;
            if (invitation.Services.FirstOrDefault() is DidCommServiceEndpoint didCommService)
            {
                DidUtils.EnsureQualifiedDid(didCommService.RecipientKeys.First());
                connectionRecord = new ConnectionRecord
                {
                    Endpoint = new AgentEndpoint {Uri = didCommService.ServiceEndpoint},
                    Alias = new ConnectionAlias {Name = invitation.Label},
                    HandshakeProtocol = HandshakeProtocol.DidExchange,
                    Role = ConnectionRole.Invitee,
                    TheirDid = DidUtils.EnsureQualifiedDid(didCommService.RecipientKeys.First()),
                    TheirVk = didCommService.RecipientKeys.First(),
                    State = ConnectionState.Invited
                };
            }
            else if (invitation.Services.FirstOrDefault() is string resolvableDid)
            {
                var recipientKey = await Did.KeyForDidAsync(await agentContext.Pool as Pool, agentContext.Wallet, resolvableDid);
                var endpointResult = await _ledgerService.LookupServiceEndpointAsync(agentContext, resolvableDid);

                connectionRecord = new ConnectionRecord
                {
                    Endpoint = new AgentEndpoint {Uri = endpointResult.Result.Endpoint},
                    Alias = new ConnectionAlias {Name = invitation.Label},
                    HandshakeProtocol = HandshakeProtocol.DidExchange,
                    Role = ConnectionRole.Invitee,
                    TheirDid = resolvableDid,
                    TheirVk = recipientKey,
                    State = ConnectionState.Invited
                };
            }
            
            if (connectionRecord == null) throw new NotImplementedException();
            
            connectionRecord.SetTag(TagConstants.ParentThreadId, invitation.GetThreadId());
            
            await _recordService.AddAsync(agentContext.Wallet, connectionRecord);

            return connectionRecord;
        }

        /// <inheritdoc/>
        public virtual async Task<(DidExchangeRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, string did)
        {
            var theirVerkey = await Did.KeyForDidAsync(await agentContext.Pool as Pool, agentContext.Wallet, did);
            var endpointResult = await _ledgerService.LookupServiceEndpointAsync(agentContext, did);
            
            var myDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");

            var connection = new ConnectionRecord
            {
                Endpoint = new AgentEndpoint {Uri = endpointResult.Result.Endpoint},
                MyDid = DidUtils.ConvertVerkeyToDidKey(myDid.VerKey),
                MyVk = myDid.VerKey,
                TheirDid = did,
                TheirVk = theirVerkey,
                State = ConnectionState.Negotiating,
            };
            
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
            request.ThreadFrom(request.Id, $"{did}#didcomm");
            
            connection.SetTag(TagConstants.LastThreadId, request.GetThreadId());
            connection.SetTag(TagConstants.ParentThreadId, request.GetParentThreadId());
            
            await _recordService.AddAsync(agentContext.Wallet, connection);

            return (request, connection);
        }

        public virtual async Task<(DidExchangeRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionRecord record)
        {
            await record.TriggerAsync(ConnectionTrigger.Request);
            
            var myDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");
            record.MyDid = DidUtils.ConvertVerkeyToDidKey(myDid.VerKey);
            record.MyVk = myDid.VerKey;

            var provisioningRecord = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
            var didDoc = new AttachmentContent
                {Base64 = record.MyDidDoc(provisioningRecord).ToJson().ToBase64Url()};
            await didDoc.SignWithJsonWebSignature(agentContext.Wallet, myDid.VerKey);
            
            var attachment = new Attachment
            {
                Id = Guid.NewGuid().ToString(),
                MimeType = "application/json",
                Data = didDoc
            };
            
            var request = new DidExchangeRequestMessage
            {
                Did = record.MyDid,
                Label = provisioningRecord.Owner.Name,
                DidDoc = attachment
            };

            var parentThread = record.GetTag(TagConstants.ParentThreadId);
            if (string.IsNullOrEmpty(parentThread)) throw new NotImplementedException();
            
            request.ThreadFrom(request.Id, parentThread);
            record.SetTag(TagConstants.LastThreadId, request.Id);

            await _recordService.UpdateAsync(agentContext.Wallet, record);

            return (request, record);
        }

        /// <inheritdoc/>
        public virtual async Task<ConnectionRecord> ProcessRequestAsync(IAgentContext agentContext,
            DidExchangeRequestMessage requestMessage, ConnectionRecord record = null)
        {
            var existingConnectionRecords = await _recordService.SearchAsync<ConnectionRecord>(agentContext.Wallet,
                SearchQuery.Equal(TagConstants.ParentThreadId, requestMessage.GetParentThreadId()));
            record ??= existingConnectionRecords?.SingleOrDefault();
            
            if(record != null)
                await record.TriggerAsync(ConnectionTrigger.Request);

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

            if (record is { } connectionRecord)
            {
                record.Alias = new ConnectionAlias {Name = requestMessage.Label};
                record.TheirDid = requestMessage.Did;
                record.TheirVk = didDoc.Keys.FirstOrDefault(key => key.Controller == requestMessage.Did)?.PublicKeyBase58
                                 ?? throw new NullReferenceException("Missing public for controller");
                record.Endpoint = agentEndpoint;
                
                record.SetTag(TagConstants.LastThreadId, requestMessage.Id);
                await _recordService.UpdateAsync(agentContext.Wallet, record);
            }
            else
            {
                record = new ConnectionRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    Alias = new ConnectionAlias {Name = requestMessage.Label},
                    TheirDid = requestMessage.Did,
                    TheirVk = didDoc.Keys.FirstOrDefault(key => key.Controller == requestMessage.Did)?.PublicKeyBase58 
                              ?? throw new NullReferenceException("Missing public for controller"),
                    Endpoint = agentEndpoint,
                    State = ConnectionState.Negotiating
                };
                
                record.SetTag(TagConstants.LastThreadId, requestMessage.GetThreadId());
                record.SetTag(TagConstants.ParentThreadId, requestMessage.GetParentThreadId());
                await _recordService.AddAsync(agentContext.Wallet, record);
            }
            
            _eventAggregator.Publish(
                new ServiceMessageProcessingEvent
                {
                    MessageType = requestMessage.Type,
                    RecordId = record.Id,
                    ThreadId = requestMessage.GetThreadId()
                });

            return record;
        }

        /// <inheritdoc/>
        public virtual async Task<(DidExchangeResponseMessage, ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Response);

            Attachment attachment = null;
            if (connectionRecord.GetTag(TagConstants.UsePublicDid) == "true")
            {
                var provisioning = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
                connectionRecord.MyDid = DidUtils.ToDid(DidUtils.DidSovMethodSpec, provisioning.IssuerDid) ?? throw new AriesFrameworkException(ErrorCode.NoPublicDid);
                connectionRecord.MyVk = provisioning.IssuerVerkey ?? throw new AriesFrameworkException(ErrorCode.NoPublicDid);
            } 
            else
            {
                var myDid = await Did.CreateAndStoreMyDidAsync(agentContext.Wallet, "{}");
                connectionRecord.MyDid = DidUtils.ConvertVerkeyToDidKey(myDid.VerKey);
                connectionRecord.MyVk = myDid.VerKey;
                
                var provisioningRecord = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
                var didDoc = new AttachmentContent
                    {Base64 = connectionRecord.MyDidDoc(provisioningRecord).ToJson().ToBase64Url()};
                await didDoc.SignWithJsonWebSignature(agentContext.Wallet, connectionRecord.MyVk);
            
                attachment = new Attachment
                {
                    Id = Guid.NewGuid().ToString(),
                    MimeType = "application/json",
                    Data = didDoc
                };
            }

            var response = new DidExchangeResponseMessage
            {
                Id = Guid.NewGuid().ToString(),
                Did = connectionRecord.MyDid,
                DidDoc = attachment
            };
            response.ThreadFrom(connectionRecord.GetTag(TagConstants.LastThreadId), connectionRecord.GetTag(TagConstants.ParentThreadId));
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);

            return (response, connectionRecord);
        }

        /// <inheritdoc/>
        public virtual async Task<ConnectionRecord> ProcessResponseAsync(IAgentContext agentContext, DidExchangeResponseMessage responseMessage, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Response);
            
            DidDoc didDoc = null;
            if (responseMessage.DidDoc?.Data?.Base64 is { } data)
            {
                var isValidSignature = await responseMessage.DidDoc.Data.VerifyJsonWebSignature();
                if (isValidSignature == false)
                    throw new AriesFrameworkException(ErrorCode.InvalidSignatureEncoding,
                        "The given JSON web signature is invalid");
                
                var json = data.FromBase64Url();
                didDoc = json.ToObject<DidDoc>();
                
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
            }

            if (didDoc == null)
            {
                if (responseMessage.Did != connectionRecord.TheirDid)
                    throw new NotImplementedException("Resolvable Dids cannot be replaced.");
            };

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
        public virtual async Task<(DidExchangeCompleteMessage, ConnectionRecord)> CreateComplete(IAgentContext agentContext, ConnectionRecord connectionRecord)
        {
            await connectionRecord.TriggerAsync(ConnectionTrigger.Complete);
            await _recordService.UpdateAsync(agentContext.Wallet, connectionRecord);

            var completeMessage = new DidExchangeCompleteMessage
            {
                Id = Guid.NewGuid().ToString()
            };
            completeMessage.ThreadFrom(connectionRecord.GetTag(TagConstants.LastThreadId), connectionRecord.GetTag(TagConstants.ParentThreadId));

            return (completeMessage, connectionRecord);
        }

        /// <inheritdoc/>
        public virtual async Task<ConnectionRecord> ProcessComplete(IAgentContext agentContext, DidExchangeCompleteMessage completeMessage,
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
        public virtual async Task<(DidExchangeProblemReportMessage, ConnectionRecord)> AbandonDidExchange(IAgentContext agentContext, ConnectionRecord connectionRecord)
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
        public virtual async Task<ConnectionRecord> ProcessProblemReportMessage(IAgentContext agentContext, DidExchangeProblemReportMessage problemReportMessage, ConnectionRecord connectionRecord)
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
