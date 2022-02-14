using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.OperationCompleted.Messages;
using Hyperledger.Aries.Models.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.OperationCompleted
{
    public class DefaultOperationCompletedHandler : IMessageHandler
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IOperationCompletedService _operationCompletedService;

        public DefaultOperationCompletedHandler(IEventAggregator eventAggregator, IOperationCompletedService operationCompletedService)
        {
            _eventAggregator = eventAggregator;
            _operationCompletedService = operationCompletedService;
        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
         {
            MessageTypesHttps.OperationCompleted
         };

        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            if (messageContext.GetMessageType() == MessageTypesHttps.OperationCompleted)
            {
                var message = messageContext.GetMessage<OperationCompletedMessage>();
                var record = await _operationCompletedService.ProcessOperationCompletedMessage(agentContext, message.Comment, messageContext.Connection.Id);
                messageContext.ContextRecord = record;

                _eventAggregator.Publish(new ServiceMessageProcessingEvent
                {
                    MessageType = MessageTypesHttps.OperationCompleted,
                    RecordId = record.Id
                });
            }
            return null;

        }
    }
}
