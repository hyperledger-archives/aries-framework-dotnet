using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <summary>
    /// Default handler for revocation notifications.
    /// </summary>
    public class DefaultRevocationNotificationHandler : IMessageHandler
    {
        private readonly IAgentContext _agentContext;
        private readonly IRevocationNotificationService _revocationNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRevocationNotificationHandler"/> class.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="revocationNotificationService">The default service for handling revocation notifications.</param>
        public DefaultRevocationNotificationHandler(
            IAgentContext agentContext,
            IRevocationNotificationService revocationNotificationService)
        {
            _agentContext = agentContext;
            _revocationNotificationService = revocationNotificationService;
        }

        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypes.RevocationNotification,
            MessageTypesHttps.RevocationNotification,
            MessageTypes.RevocationNotificationAcknowledgement, 
            MessageTypesHttps.RevocationNotificationAcknowledgement
        };

        /// <summary>
        /// Processes the revocation notifications message
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="messageContext">The message context.</param>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                case MessageTypes.RevocationNotification:
                case MessageTypesHttps.RevocationNotification:
                {
                    var revocationNotificationMessage = messageContext.GetMessage<RevocationNotificationMessage>();
                    await _revocationNotificationService.ProcessRevocationNotificationAsync(
                        _agentContext, revocationNotificationMessage);
                    return null;
                }    

                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
                
                case MessageTypes.RevocationNotificationAcknowledgement:
                case MessageTypesHttps.RevocationNotificationAcknowledgement:
                    await _revocationNotificationService.ProcessRevocationNotificationAcknowledgementAsync(
                        _agentContext, messageContext.GetMessage<RevocationNotificationAcknowledgeMessage>());
                    return null;
            }
        }
    }
}
