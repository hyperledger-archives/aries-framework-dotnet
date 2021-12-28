using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Models.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.ProblemReport
{
    public class DefaultProblemReportHandler : IMessageHandler
    {
        private readonly IEventAggregator EventAggregator;

        public DefaultProblemReportHandler(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypesHttps.ProblemReportMessageType
        };

        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            if (messageContext.GetMessageType() == MessageTypesHttps.ProblemReportMessageType)
            {
                var message = messageContext.GetMessage<ProblemReportMessage>();

                EventAggregator.Publish(new ServiceMessageProcessingEvent
                {
                    MessageType = MessageTypesHttps.ProblemReportMessageType,
                    ThreadId = message.FindDecorator<ThreadDecorator>("thread")?.ThreadId,
                    RecordId = message.Id
                });
            }

            return null;
        }
    }
}
