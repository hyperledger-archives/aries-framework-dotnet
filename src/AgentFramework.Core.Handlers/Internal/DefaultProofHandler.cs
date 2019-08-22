using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Proofs;

namespace AgentFramework.Core.Handlers.Internal
{
    internal class DefaultProofHandler : IMessageHandler
    {
        private readonly IProofService _proofService;

        public DefaultProofHandler(IProofService proofService)
        {
            _proofService = proofService;
        }

        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        public IEnumerable<MessageType> SupportedMessageTypes => new[]
        {
            new MessageType(MessageTypes.ProofRequest),
            new MessageType(MessageTypes.DisclosedProof)
        };

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="messageContext">The agent message agentContext.</param>
        /// <returns></returns>
        /// <exception cref="AgentFrameworkException">Unsupported message type {messageType}</exception>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, MessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                case MessageTypes.ProofRequest:
                    var request = messageContext.GetMessage<ProofRequestMessage>();
                    await _proofService.ProcessProofRequestAsync(agentContext, request, messageContext.Connection);
                    break;

                case MessageTypes.DisclosedProof:
                    var proof = messageContext.GetMessage<ProofMessage>();
                    await _proofService.ProcessProofAsync(agentContext, proof);
                    break;
                default:
                    throw new AgentFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
            return null;
        }
    }
}
