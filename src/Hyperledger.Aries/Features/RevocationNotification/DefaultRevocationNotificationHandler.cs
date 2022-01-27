using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    internal class DefaultRevocationNotificationHandler : IMessageHandler
    {
        public DefaultRevocationNotificationHandler()
        {

        }

        public IEnumerable<MessageType> SupportedMessageTypes => new MessageType[]
        {
            MessageTypes.IssueCredentialNames.RevocationNotification,
            MessageTypesHttps.IssueCredentialNames.RevocationNotification
        };

        public Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            switch (messageContext.GetMessageType())
            {
                case MessageTypes.IssueCredentialNames.RevocationNotification:
                case MessageTypesHttps.IssueCredentialNames.RevocationNotification:
                    return null;    

                default:
                    throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                        $"Unsupported message type {messageContext.GetMessageType()}");
            }
        }
    }
}
