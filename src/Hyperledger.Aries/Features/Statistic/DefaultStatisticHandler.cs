using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.Statistic.Messages;
using Hyperledger.Aries.Models.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.Statistic
{
    public class DefaultStatisticHandler : IMessageHandler
    {
        private readonly IStatisticService StatisticService;
        private readonly IEventAggregator EventAggregator;

        public DefaultStatisticHandler(
            IStatisticService statisticService,
            IEventAggregator eventAggregator    
        )
        {
            StatisticService = statisticService;
            EventAggregator = eventAggregator;
        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypesHttps.StatisticNames.ProofPresentation
        };

        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            if (messageContext.GetMessageType() == MessageTypesHttps.StatisticNames.ProofPresentation)
            {
                var message = messageContext.GetMessage<PresentProofStatisticMessage>();
                var record = await StatisticService.ProcessPresentProof(agentContext, message, messageContext.Connection);
                messageContext.ContextRecord = record;

                EventAggregator.Publish(new ServiceMessageProcessingEvent
                {
                    MessageType = MessageTypesHttps.StatisticNames.ProofPresentation,
                    ThreadId = message.FindDecorator<ThreadDecorator>("thread")?.ThreadId,
                    RecordId = message.Id
                });
            }

            return null;
        }
    }
}
