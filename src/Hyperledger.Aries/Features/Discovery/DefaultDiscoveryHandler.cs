﻿using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.Discovery
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

        protected override Task<AgentMessage> ProcessAsync(DiscoveryQueryMessage message, IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            return Task.FromResult<AgentMessage>(_discoveryService.CreateQueryResponse(agentContext, message));
        }
    }
}
