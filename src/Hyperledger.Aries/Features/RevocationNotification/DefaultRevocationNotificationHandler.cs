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
        private readonly IRevocationNotificationService _revocationNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRevocationNotificationHandler"/> class.
        /// </summary>
        /// <param name="revocationNotificationService">The default service for handling revocation notifications.</param>
        public DefaultRevocationNotificationHandler(
            IRevocationNotificationService revocationNotificationService)
        {
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
            MessageTypesHttps.RevocationNotification,
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
                case MessageTypesHttps.RevocationNotification:
                {
                    var revocationNotificationMessage = messageContext.GetMessage<RevocationNotificationMessage>();
                    var acknowledgeMessage = await _revocationNotificationService.ProcessRevocationNotificationAsync(
                        agentContext, revocationNotificationMessage);
                    return acknowledgeMessage;
                }    
                
                case MessageTypesHttps.RevocationNotificationAcknowledgement:
                    await _revocationNotificationService.ProcessRevocationNotificationAcknowledgementAsync(
                        agentContext, messageContext.GetMessage<RevocationNotificationAcknowledgeMessage>());
                    return null;
                
                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
        }
    }
}
