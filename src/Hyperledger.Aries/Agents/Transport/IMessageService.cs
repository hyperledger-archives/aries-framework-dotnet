using System.Threading.Tasks;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Router service.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Sends the agent message to the endpoint asynchronously.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message.</param>
        /// <param name="recipientKey">The recipients key.</param>
        /// <param name="endpointUri">The destination endpoint.</param>
        /// <param name="routingKeys">The routing keys.</param>
        /// <param name="senderKey">The senders key.</param>
        /// <returns>The response as a message context object if return routing requested async.</returns>
        Task SendAsync(Wallet wallet, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null);

        /// <summary>
        /// Sends the message and receives a response by adding return routing decorator
        /// according to the Routing RFC.
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="message"></param>
        /// <param name="recipientKey"></param>
        /// <param name="endpointUri"></param>
        /// <param name="routingKeys"></param>
        /// <param name="senderKey"></param>
        /// <returns></returns>
        Task<MessageContext> SendReceiveAsync(Wallet wallet, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null);

    }
}
