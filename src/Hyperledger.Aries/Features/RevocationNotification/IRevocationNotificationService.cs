using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <summary>
    ///  Default service for revocation notifications
    /// </summary>
    public interface IRevocationNotificationService
    {
        /// <summary>
        ///  Processes an incoming revocation notification message. If a please ACK decorator is present 
        ///  the method will send acknowledge messages according to the decorator.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="revocationNotificationMessage">The incoming revocation notification message.</param>
        public Task<RevocationNotificationAcknowledgeMessage> ProcessRevocationNotificationAsync(
            IAgentContext agentContext,
            RevocationNotificationMessage revocationNotificationMessage);

        /// <summary>
        ///  Processes an incoming revocation notification acknowledgement message.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="revocationNotificationAcknowledgeMessage">The incoming revocation notification acknowledgement message.</param>
        public Task ProcessRevocationNotificationAcknowledgementAsync(
            IAgentContext agentContext,
            RevocationNotificationAcknowledgeMessage revocationNotificationAcknowledgeMessage);
    }
}
