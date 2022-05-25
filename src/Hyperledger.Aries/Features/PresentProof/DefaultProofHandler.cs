using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.PresentProof.Messages;

namespace Hyperledger.Aries.Features.PresentProof
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
        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypes.PresentProofNames.AcknowledgePresentation,
            MessageTypes.PresentProofNames.ProposePresentation,
            MessageTypes.PresentProofNames.Presentation,
            MessageTypes.PresentProofNames.RequestPresentation,
            MessageTypesHttps.PresentProofNames.AcknowledgePresentation,
            MessageTypesHttps.PresentProofNames.ProposePresentation,
            MessageTypesHttps.PresentProofNames.Presentation,
            MessageTypesHttps.PresentProofNames.RequestPresentation
        };

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="messageContext">The agent message agentContext.</param>
        /// <returns></returns>
        /// <exception cref="AriesFrameworkException">Unsupported message type {messageType}</exception>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                // v1.0
                case MessageTypesHttps.PresentProofNames.ProposePresentation:
                case MessageTypes.PresentProofNames.ProposePresentation:
                {
                    var message = messageContext.GetMessage<ProposePresentationMessage>();
                    var record = await _proofService.ProcessProposalAsync(agentContext, message, messageContext.Connection);

                    messageContext.ContextRecord = record;
                    break;
                }
                case MessageTypes.PresentProofNames.RequestPresentation:
                case MessageTypesHttps.PresentProofNames.RequestPresentation:
                {
                    var message = messageContext.GetMessage<RequestPresentationMessage>();
                    var record = await _proofService.ProcessRequestAsync(agentContext, message, messageContext.Connection);

                    messageContext.ContextRecord = record;
                    break;
                }
                case MessageTypes.PresentProofNames.Presentation:
                case MessageTypesHttps.PresentProofNames.Presentation:
                {
                    var message = messageContext.GetMessage<PresentationMessage>();
                    var record = await _proofService.ProcessPresentationAsync(agentContext, message);

                    messageContext.ContextRecord = record;
                    break;
                }

                case MessageTypes.PresentProofNames.AcknowledgePresentation:
                case MessageTypesHttps.PresentProofNames.AcknowledgePresentation:
                {
                    var message = messageContext.GetMessage<PresentationAcknowledgeMessage>();
                    var record = await _proofService.ProcessAcknowledgeMessageAsync(agentContext, message);

                    messageContext.ContextRecord = record;
                    break;
                }
                
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
            return null;
        }
    }
}
