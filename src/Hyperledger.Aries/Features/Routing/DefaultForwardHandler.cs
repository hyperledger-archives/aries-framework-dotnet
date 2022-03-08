using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Connection;

namespace Hyperledger.Aries.Features.Routing
{
    internal class DefaultForwardHandler : MessageHandlerBase<ForwardMessage>
    {
        private readonly IConnectionService _connectionService;

        public DefaultForwardHandler(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public override IEnumerable<MessageType> SupportedMessageTypes => new MessageType[] { MessageTypes.Forward, MessageTypesHttps.Forward };

        protected override async Task<AgentMessage> ProcessAsync(ForwardMessage message, IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            var connectionRecord = await _connectionService.ResolveByMyKeyAsync(agentContext, message.To);

            if (agentContext is DefaultAgentContext context)
                context.AddNext(new PackedMessageContext(message.Message.ToJson(), connectionRecord));

            return null;
        }
    }
}
