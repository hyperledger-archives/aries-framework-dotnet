using System.Threading.Tasks;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Records;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Router service.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Prepares a wire level message from the application level agent message for a connection asynchronously 
        /// this includes packing the message and wrapping it in required forward messages
        /// if the message requires it.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message context.</param>
        /// <param name="connection">The connection to prepare the message for.</param>
        /// <param name="recipientKey">The recipients verkey to encrypt the message for.</param>
        /// <param name="routing">Prepare the message with routing, i.e pack the message with embedded forward messages.</param>
        /// <returns>The response async.</returns>
        Task<byte[]> PrepareAsync(Wallet wallet, AgentMessage message, ConnectionRecord connection, string recipientKey = null, bool routing = true);

        /// <summary>
        /// Prepares a wire level message from the application level agent message asynchronously 
        /// this includes packing the message and wrapping it in required forward messages
        /// if the message requires it.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message context.</param>
        /// <param name="recipientKey">The key to encrypt the message for.</param>
        /// <param name="routingKeys">The routing keys to pack the message for.</param>
        /// <param name="senderKey">The sender key to encrypt the message from.</param>
        /// <returns>The response async.</returns>
        Task<byte[]> PrepareAsync(Wallet wallet, AgentMessage message, string recipientKey, string[] routingKeys = null,
            string senderKey = null);

        /// <summary>
        /// Sends the agent message asynchronously.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message.</param>
        /// <param name="connection">The connection record.</param>
        /// <param name="recipientKey">The recipients verkey to encrypt the message for.</param>
        /// <param name="requestResponse">Request response message.</param>
        /// <returns>The response as a message context object if return routing requested async.</returns>
        Task<MessageContext> SendAsync(Wallet wallet, AgentMessage message, ConnectionRecord connection, string recipientKey = null, bool requestResponse = false);

        /// <summary>
        /// Sends the agent message to the endpoint asynchronously.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="message">The message.</param>
        /// <param name="recipientKey">The recipients key.</param>
        /// <param name="endpointUri">The destination endpoint.</param>
        /// <param name="routingKeys">The routing keys.</param>
        /// <param name="senderKey">The senders key.</param>
        /// <param name="requestResponse">Request response message.</param>
        /// <returns>The response as a message context object if return routing requested async.</returns>
        Task<MessageContext> SendAsync(Wallet wallet, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null, bool requestResponse = false);

    }
}
