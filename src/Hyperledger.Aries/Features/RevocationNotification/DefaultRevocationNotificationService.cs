using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Decorators.PleaseAck;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Events;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <inheritdoc />
    public class DefaultRevocationNotificationService : IRevocationNotificationService
    {
        private readonly IAgentContext _agentContext;
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageService _messageService;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DefaultRevocationNotificationService"/> class.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="connectionService">The connection service.</param>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="messageService">The message service.</param>
        public DefaultRevocationNotificationService(
            IAgentContext agentContext,
            IConnectionService connectionService,
            ICredentialService credentialService,
            IEventAggregator eventAggregator,
            IMessageService messageService)
        {
            _agentContext = agentContext;
            _connectionService = connectionService;
            _credentialService = credentialService;
            _eventAggregator = eventAggregator;
            _messageService = messageService;
        }
        
        /// <inheritdoc />
        public async Task ProcessRevocationNotificationAsync(
            IAgentContext agentContext,
            RevocationNotificationMessage revocationNotificationMessage)
        {
            var plsAckDec = revocationNotificationMessage.FindDecorator<PleaseAckDecorator>(
                DecoratorNames.PleaseAckDecorator);
            
            var credentialRecord = await _credentialService.GetByThreadIdAsync(
                _agentContext, revocationNotificationMessage.ThreadId);

            if (plsAckDec != null)
            {
                var acknowledgeMessage = new RevocationNotificationAcknowledgeMessage(false)
                {
                    Id = revocationNotificationMessage.ThreadId,
                    Status = AcknowledgementStatusConstants.Ok
                };    
                acknowledgeMessage.ThreadFrom(revocationNotificationMessage);

                var connectionRecord = 
                    await _connectionService.GetByThreadIdAsync(_agentContext, acknowledgeMessage.Id);
                
                if (plsAckDec.On.Contains(OnValues.RECEIPT))
                    await _messageService.SendAsync(_agentContext, acknowledgeMessage, connectionRecord);
                
                _eventAggregator.Publish(new ServiceMessageProcessingEvent
                {
                    RecordId = credentialRecord.Id, 
                    MessageType = MessageTypes.RevocationNotification,
                    ThreadId = revocationNotificationMessage.ThreadId
                });
                
                if (plsAckDec.On.Contains(OnValues.OUTCOME)) 
                    await _messageService.SendAsync(_agentContext, acknowledgeMessage, connectionRecord);

                return;
            }
            
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = MessageTypes.RevocationNotification,
                ThreadId = revocationNotificationMessage.ThreadId
            });
        }

        /// <inheritdoc />
        public async Task ProcessRevocationNotificationAcknowledgementAsync(
            IAgentContext agentContext,
            RevocationNotificationAcknowledgeMessage revocationNotificationAcknowledgeMessage)
        {
            var credentialRecord = await _credentialService.GetByThreadIdAsync(
                _agentContext, revocationNotificationAcknowledgeMessage.Id);
            
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = MessageTypes.RevocationNotification,
                ThreadId = revocationNotificationAcknowledgeMessage.GetThreadId()
            });
        }
    }
}
