using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.Statistic.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.Statistic
{
    public class DefaultStatisticHandler : IMessageHandler
    {
        private readonly IStatisticService StatisticService;

        public DefaultStatisticHandler(IStatisticService statisticService)
        {
            StatisticService = statisticService;
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
            }

            return null;
        }
    }
}
