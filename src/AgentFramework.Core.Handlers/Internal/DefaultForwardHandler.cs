using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Routing;

namespace AgentFramework.Core.Handlers.Internal
{
    internal class DefaultForwardHandler : MessageHandlerBase<ForwardMessage>
    {
        private readonly IConnectionService _connectionService;

        public DefaultForwardHandler(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        protected override async Task<AgentMessage> ProcessAsync(ForwardMessage message, IAgentContext agentContext, MessageContext messageContext)
        {
            var connectionRecord = await _connectionService.ResolveByMyKeyAsync(agentContext, message.To);

            if (agentContext is AgentContext context) 
                context.AddNext(new MessageContext(message.Message, true, connectionRecord));

            return null;
        }
    }
}
