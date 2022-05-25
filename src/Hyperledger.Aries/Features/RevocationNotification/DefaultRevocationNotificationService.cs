using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Decorators.PleaseAck;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <inheritdoc />
    public class DefaultRevocationNotificationService : IRevocationNotificationService
    {
        private readonly ICredentialService _credentialService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWalletRecordService _recordService;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DefaultRevocationNotificationService"/> class.
        /// </summary>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="recordService">The record service.</param>
        public DefaultRevocationNotificationService(
            ICredentialService credentialService,
            IEventAggregator eventAggregator,
            IWalletRecordService recordService)
        {
            _credentialService = credentialService;
            _eventAggregator = eventAggregator;
            _recordService = recordService;
        }
        
        /// <inheritdoc />
        public virtual async Task<RevocationNotificationAcknowledgeMessage> ProcessRevocationNotificationAsync(
            IAgentContext agentContext,
            RevocationNotificationMessage revocationNotificationMessage)
        {
            RevocationNotificationAcknowledgeMessage result = null;
            
            var plsAckDec = revocationNotificationMessage.FindDecorator<PleaseAckDecorator>(
                DecoratorNames.PleaseAckDecorator);
            
            var credentialRecord = await _credentialService.GetByThreadIdAsync(
                agentContext, revocationNotificationMessage.ThreadId);

            if (plsAckDec != null)
            {
                var acknowledgeMessage = new RevocationNotificationAcknowledgeMessage
                {
                    Id = revocationNotificationMessage.ThreadId,
                    Status = AcknowledgementStatusConstants.Ok
                };    
                acknowledgeMessage.ThreadFrom(revocationNotificationMessage);

                if (plsAckDec.On.Contains(OnValues.OUTCOME) || plsAckDec.On.Contains(OnValues.RECEIPT))
                    result = acknowledgeMessage;
            }
            
            await credentialRecord.TriggerAsync(CredentialTrigger.Revoke);
            await _recordService.UpdateAsync(agentContext.Wallet, credentialRecord);
            _eventAggregator.Publish(new ServiceMessageProcessingEvent
            {
                RecordId = credentialRecord.Id,
                MessageType = MessageTypesHttps.RevocationNotification,
                ThreadId = revocationNotificationMessage.ThreadId
            });

            return result;
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
                MessageType = MessageTypesHttps.RevocationNotificationAcknowledgement,
                ThreadId = revocationNotificationAcknowledgeMessage.GetThreadId()
            });
        }
    }
}
