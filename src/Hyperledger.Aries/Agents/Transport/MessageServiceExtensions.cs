using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Utils;

namespace Hyperledger.Aries.Agents
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
        /// <param name="agentContext"></param>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task SendAsync(this IMessageService service, IAgentContext agentContext, AgentMessage message, ConnectionRecord connection)
        {
            var routingKeys = connection.Endpoint?.Verkey != null ? connection.Endpoint.Verkey : new string[0];
            var recipientKey = connection.TheirVk ?? connection.GetTag(TagConstants.InvitationKey) ?? throw new InvalidOperationException("Cannot locate a recipient key");

            if (connection.Endpoint?.Uri == null)
                throw new AriesFrameworkException(ErrorCode.A2AMessageTransmissionError, "Cannot send to connection that does not have endpoint information specified");

            await service.SendAsync(agentContext, message, recipientKey, connection.Endpoint.Uri, routingKeys, connection.MyVk);
        }

        /// <summary>
        /// Sends the message and receives a response by adding return routing decorator
        /// according to the Routing RFC.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="agentContext"></param>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task<MessageContext> SendReceiveAsync(this IMessageService service, IAgentContext agentContext, AgentMessage message, ConnectionRecord connection)
        {
            var routingKeys = connection.Endpoint?.Verkey != null ? connection.Endpoint.Verkey : new string[0];
            var recipientKey = connection.TheirVk ?? connection.GetTag(TagConstants.InvitationKey) ?? throw new InvalidOperationException("Cannot locate a recipient key");

            if (connection.Endpoint?.Uri == null)
                throw new AriesFrameworkException(ErrorCode.A2AMessageTransmissionError, "Cannot send to connection that does not have endpoint information specified");

            return await service.SendReceiveAsync(agentContext, message, recipientKey, connection.Endpoint.Uri, routingKeys, connection.MyVk);
        }

        /// <summary>
        /// Sends the message and receives a response by adding return routing decorator
        /// according to the Routing RFC. It also tries to cast the response to an expected
        /// type of <see cref="AgentMessage" />
        /// </summary>
        /// <param name="service"></param>
        /// <param name="agentContext"></param>
        /// <param name="message"></param>
        /// <param name="connection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> SendReceiveAsync<T>(this IMessageService service, IAgentContext agentContext, AgentMessage message, ConnectionRecord connection)
            where T : AgentMessage, new()
        {
            var response = await service.SendReceiveAsync(agentContext, message, connection);
            if (response is UnpackedMessageContext unpackedContext)
            {
                return unpackedContext.GetMessage<T>();
            }
            throw new InvalidOperationException("Couldn't cast the message to the expexted type or response was invalid");
        }

        /// <summary>
        /// Sends the message and receives a response by adding return routing decorator
        /// according to the Routing RFC. It also tries to cast the response to an expected
        /// type of <see cref="AgentMessage" />
        /// </summary>
        /// <param name="service"></param>
        /// <param name="agentContext"></param>
        /// <param name="message"></param>
        /// <param name="recipientKey"></param>
        /// <param name="endpointUri"></param>
        /// <param name="routingKeys"></param>
        /// <param name="senderKey"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> SendReceiveAsync<T>(this IMessageService service, IAgentContext agentContext, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null)
            where T : AgentMessage, new()
        {
            var response = await service.SendReceiveAsync(agentContext, message, recipientKey, endpointUri, routingKeys, senderKey);
            if (response is UnpackedMessageContext unpackedContext)
            {
                return unpackedContext.GetMessage<T>();
            }
            throw new InvalidOperationException("Couldn't cast the message to the expexted type or response was invalid");
        }
    }
}
