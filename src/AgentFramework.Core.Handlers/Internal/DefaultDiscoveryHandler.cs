using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Discovery;

namespace AgentFramework.Core.Handlers.Internal
{
    /// <summary>
    /// Default discovery message handler.
    /// </summary>
    internal class DefaultDiscoveryHandler : MessageHandlerBase<DiscoveryQueryMessage>
    {
        private readonly IDiscoveryService _discoveryService;

        public DefaultDiscoveryHandler(IDiscoveryService discoveryService)
        {
            _discoveryService = discoveryService;
        }

        protected override Task<AgentMessage> ProcessAsync(DiscoveryQueryMessage message, IAgentContext agentContext, MessageContext messageContext)
        {
            return Task.FromResult<AgentMessage>(_discoveryService.CreateQueryResponse(agentContext, message));
        }
    }
}
