using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Decorators.PleaseAck;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <inheritdoc />
    public class DefaultRevocationNotificationService : IRevocationNotificationService
    {
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageService _messageService;
        private readonly IWalletRecordService _recordService;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DefaultRevocationNotificationService"/> class.
        /// </summary>
        /// <param name="connectionService">The connection service.</param>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="messageService">The message service.</param>
        /// <param name="recordService">The record service.</param>
        public DefaultRevocationNotificationService(
            IConnectionService connectionService,
            ICredentialService credentialService,
            IEventAggregator eventAggregator,
            IMessageService messageService,
            IWalletRecordService recordService)
        {
            _connectionService = connectionService;
            _credentialService = credentialService;
            _eventAggregator = eventAggregator;
            _messageService = messageService;
            _recordService = recordService;
        }
        
        /// <inheritdoc />
        public virtual async Task ProcessRevocationNotificationAsync(
            IAgentContext agentContext,
            RevocationNotificationMessage revocationNotificationMessage)
        {
            var plsAckDec = revocationNotificationMessage.FindDecorator<PleaseAckDecorator>(
                DecoratorNames.PleaseAckDecorator);
            
            var credentialRecord = await _credentialService.GetByThreadIdAsync(
                agentContext, revocationNotificationMessage.ThreadId);

            if (plsAckDec != null)
            {
                var acknowledgeMessage = new RevocationNotificationAcknowledgeMessage(false)
                {
                    Id = revocationNotificationMessage.ThreadId,
                    Status = AcknowledgementStatusConstants.Ok
                };    
                acknowledgeMessage.ThreadFrom(revocationNotificationMessage);

                var connectionRecord = await _connectionService.GetAsync(agentContext, credentialRecord.ConnectionId);

                await credentialRecord.TriggerAsync(CredentialTrigger.Revoke);
                await _recordService.UpdateAsync(agentContext.Wallet, credentialRecord);
                _eventAggregator.Publish(new ServiceMessageProcessingEvent
                {
                    RecordId = credentialRecord.Id, 
                    MessageType = MessageTypes.RevocationNotification,
                    ThreadId = revocationNotificationMessage.ThreadId
                });
                
                if (plsAckDec.On.Contains(OnValues.OUTCOME) || plsAckDec.On.Contains(OnValues.RECEIPT)) 
                    await _messageService.SendAsync(agentContext, acknowledgeMessage, connectionRecord);

                return;
            }
            
            await credentialRecord.TriggerAsync(CredentialTrigger.Revoke);
            await _recordService.UpdateAsync(agentContext.Wallet, credentialRecord);
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = MessageTypes.RevocationNotification,
                ThreadId = revocationNotificationMessage.ThreadId
            });
        }

        /// <inheritdoc />
        public virtual async Task ProcessRevocationNotificationAcknowledgementAsync(
            IAgentContext agentContext,
            RevocationNotificationAcknowledgeMessage revocationNotificationAcknowledgeMessage)
        {
            var credentialRecord = await _credentialService.GetByThreadIdAsync(
                agentContext, revocationNotificationAcknowledgeMessage.Id);
            
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = MessageTypes.RevocationNotificationAcknowledgement,
                ThreadId = revocationNotificationAcknowledgeMessage.GetThreadId()
            });
        }
    }
}
