using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Records;
using Hyperledger.Indy.WalletApi;

namespace System
{
    /// <summary>
    /// Message Service Extensions
    /// </summary>
    public static class MessageServiceExtensions
    {
        /// <summary>
        /// Sends the agent message to the endpoint asynchronously.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="wallet"></param>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task SendAsync(this IMessageService service, Wallet wallet, AgentMessage message, ConnectionRecord connection)
        {
            var routingKeys = connection.Endpoint?.Verkey != null ? new[] { connection.Endpoint.Verkey } : new string[0];
            var recipientKey = connection.TheirVk ?? connection.GetTag("InvitationKey") ?? throw new InvalidOperationException("Cannot locate a recipient key");

            if (connection.Endpoint?.Uri == null)
                throw new AgentFrameworkException(ErrorCode.A2AMessageTransmissionError, "Cannot send to connection that does not have endpoint information specified");

            await service.SendAsync(wallet, message, recipientKey, connection.Endpoint.Uri, routingKeys, connection.MyVk);
        }

        /// <summary>
        /// Sends the message and receives a response by adding return routing decorator
        /// according to the Routing RFC.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="wallet"></param>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task<MessageContext> SendReceiveAsync(this IMessageService service, Wallet wallet, AgentMessage message, ConnectionRecord connection)
        {
            var routingKeys = connection.Endpoint?.Verkey != null ? new[] { connection.Endpoint.Verkey } : new string[0];
            var recipientKey = connection.TheirVk ?? connection.GetTag("InvitationKey") ?? throw new InvalidOperationException("Cannot locate a recipient key");

            if (connection.Endpoint?.Uri == null)
                throw new AgentFrameworkException(ErrorCode.A2AMessageTransmissionError, "Cannot send to connection that does not have endpoint information specified");

            return await service.SendReceiveAsync(wallet, message, recipientKey, connection.Endpoint.Uri, routingKeys, connection.MyVk);
        }
    }
}