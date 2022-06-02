using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.Handshakes;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Common.Dids;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;

namespace Hyperledger.Aries.Features.OutOfBand
{
    /// <inheritdoc />
    public class DefaultOutOfBandService : IOutOfBandService
    {
        private IEventAggregator _eventAggregator;
        private IConnectionService _connectionService;
        private IDidExchangeService _didExchangeService;
        private IProvisioningService _provisioningService;
        private IWalletRecordService _recordService;

        public DefaultOutOfBandService(IEventAggregator eventAggregator, IConnectionService connectionService, IProvisioningService provisioningService, IDidExchangeService didExchangeService, IWalletRecordService recordService)
        {
            _eventAggregator = eventAggregator;
            _connectionService = connectionService;
            _provisioningService = provisioningService;
            _didExchangeService = didExchangeService;
            _recordService = recordService;
        }

        /// <inheritdoc />
        public async Task<(InvitationMessage, ConnectionRecord)> CreateInvitationAsync(IAgentContext agentContext, IEnumerable<AgentMessage> requests = null, InviteConfiguration config = null)
        {
            config ??= new InviteConfiguration();
            var (_, record) = await _connectionService.CreateInvitationAsync(agentContext, config);
            var provisioningRecord = await _provisioningService.GetProvisioningAsync(agentContext.Wallet);
            
            var invitation = new InvitationMessage
            {
                Label = config.MyAlias.Name ?? provisioningRecord.Owner.Name ?? string.Empty,
                GoalCode = string.Empty,
                Goal = string.Empty,
                Accept = new []{ MediaTypes.EncryptionEnvelopeV1 },
                HandshakeProtocols = new []{ HandshakeProtocolUri.DidExchange, HandshakeProtocolUri.Connections }
            };
            
            if (config.UsePublicDid)
            {
                if (provisioningRecord.IssuerDid == null)
                    throw new AriesFrameworkException(ErrorCode.NoPublicDid, "No public did was provisioned.");
                invitation.Services = new object[] { DidUtils.ToDid(DidUtils.DidSovMethodSpec, provisioningRecord.IssuerDid) };
                record.SetTag(TagConstants.UsePublicDid, "true");
            }
            else
            {
                invitation.Services = new object[] {record.MyDidCommService(provisioningRecord)};
            }
            invitation.AddRequests(requests);

            var parentThreadId = invitation.GetThreadId();
            record.SetTag(TagConstants.ParentThreadId, parentThreadId);
            
            await _recordService.UpdateAsync(agentContext.Wallet, record);

            return (invitation, record);
        }

        /// <inheritdoc />
        public async Task<(HandshakeReuseMessage, ConnectionRecord)> ProcessInvitationMessage(IAgentContext agentContext, InvitationMessage invitation)
        {
            if (invitation.Accept.Contains("didcomm/aip2;env=rfc19") == false)
                throw new NotImplementedException("The agent only supports encryption envelope v1.");
            
            ConnectionRecord connectionRecord = null;
            HandshakeReuseMessage reuseMessage = null;
            
            var service = invitation.Services.First();
            string theirDid = null; 
            if (service is string resolvableDid)
            {
                theirDid = resolvableDid;
            }
            if (service is DidCommServiceEndpoint didComm)
            {
                theirDid = didComm.RecipientKeys.FirstOrDefault();
            }
            theirDid = DidUtils.EnsureQualifiedDid(theirDid);
            
            var existingConnection = (await _connectionService.ListAsync(agentContext,
                SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), theirDid))).FirstOrDefault();
            
            if (existingConnection != null && invitation.AttachedRequests == null)
            {
                var message = new HandshakeReuseMessage
                {
                    Id = invitation.Id
                };
                message.ThreadFrom(message.Id, invitation.Id);

                reuseMessage = message;
                connectionRecord = existingConnection;
            } 
            else if (invitation.HandshakeProtocols != null && invitation.HandshakeProtocols.Length > 0)
            {
                connectionRecord = await InitiateHandshakeProtocol(agentContext, invitation);
            }
            
            // Todo: Handle attached requests

            _eventAggregator.Publish(new ServiceMessageProcessingEvent()
            {
                MessageType = invitation.Type,
                RecordId = connectionRecord?.Id,
                ThreadId = invitation.GetThreadId()
            });

            return (reuseMessage, connectionRecord);
        }

        /// <inheritdoc />
        public async Task<HandshakeReuseAcceptedMessage> ProcessHandshakeReuseMessage(IAgentContext agentContext, HandshakeReuseMessage handshakeReuseMessage)
        {
            var accept = new HandshakeReuseAcceptedMessage();
            accept.ThreadFrom(handshakeReuseMessage);
            
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                MessageType = handshakeReuseMessage.Type,
                ThreadId = handshakeReuseMessage.GetThreadId()
            });

            return accept;
        }

        /// <inheritdoc />
        public Task ProcessHandshakeReuseAccepted(IAgentContext agentContext,
            HandshakeReuseAcceptedMessage handshakeReuseAcceptedMessage)
        {
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                MessageType = handshakeReuseAcceptedMessage.Type,
                ThreadId = handshakeReuseAcceptedMessage.GetThreadId()
            });

            return Task.CompletedTask;
        }

        private Task<ConnectionRecord> InitiateHandshakeProtocol(IAgentContext agentContext, InvitationMessage invitation)
        {
            HandshakeProtocol? selectedHandshake =
                GetFirstSupportedHandshakeProtocol(invitation.HandshakeProtocols);
            
            switch (selectedHandshake)
            {
                case HandshakeProtocol.Connections:
                    return _connectionService.ProcessInvitationAsync(agentContext, invitation);
                case HandshakeProtocol.DidExchange:
                    return _didExchangeService.ProcessInvitationAsync(agentContext, invitation);
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        "Unsupported handshake protocol in out-of-band message");
            }
        }

        private HandshakeProtocol? GetFirstSupportedHandshakeProtocol(string[] handshakes)
        {
            foreach (var handshakeProtocol in handshakes)
            {
                switch (handshakeProtocol)
                {
                    case HandshakeProtocolUri.Connections:
                        return HandshakeProtocol.Connections;
                    case HandshakeProtocolUri.DidExchange:
                        return HandshakeProtocol.DidExchange;
                }
            }

            return null;
        }
    }
}
