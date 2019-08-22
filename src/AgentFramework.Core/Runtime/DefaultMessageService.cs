using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Decorators.Transport;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Routing;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultMessageService : IMessageService
    {
        /// <summary>The agent wire message MIME type</summary>
        public const string AgentWireMessageMimeType = "application/ssi-agent-wire";
        
        /// <summary>The logger</summary>
        // ReSharper disable InconsistentNaming
        protected readonly ILogger<DefaultMessageService> Logger;

        /// <summary>The HTTP client</summary>
        protected readonly IEnumerable<IMessageDispatcher> MessageDispatchers;
        // ReSharper restore InconsistentNaming

        /// <summary>Initializes a new instance of the <see cref="DefaultMessageService"/> class.</summary>
        /// <param name="logger">The logger.</param>
        /// <param name="messageDispatchers">The message handler.</param>
        public DefaultMessageService(
            ILogger<DefaultMessageService> logger, 
            IEnumerable<IMessageDispatcher> messageDispatchers)
        {
            Logger = logger;
            MessageDispatchers = messageDispatchers;
        }

        /// <inheritdoc />
        public virtual Task<byte[]> PrepareAsync(Wallet wallet, AgentMessage message, ConnectionRecord connection, string recipientKey = null, bool useRoutingKeys = true)
        {
            recipientKey = recipientKey
                                ?? connection.TheirVk
                                ?? throw new AgentFrameworkException(
                                    ErrorCode.A2AMessageTransmissionError, "Cannot find encryption key");

            var routingKeys = useRoutingKeys && connection.Endpoint?.Verkey != null ? new[] { connection.Endpoint.Verkey } : new string[0];

            return PrepareAsync(wallet, message, recipientKey, routingKeys, connection.MyVk);
        }

        /// <inheritdoc />
        public virtual async Task<byte[]> PrepareAsync(Wallet wallet, AgentMessage message, string recipientKey, string[] routingKeys = null, string senderKey = null)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (recipientKey == null) throw new ArgumentNullException(nameof(recipientKey));

            // Pack application level message
            var msg = await CryptoUtils.PackAsync(wallet, recipientKey, message.ToByteArray(), senderKey);

            var previousKey = recipientKey;

            if (routingKeys != null)
            {
                // TODO: In case of multiple key, should they each wrap a forward message
                // or pass all keys to the PackAsync function as array?
                foreach (var routingKey in routingKeys)
                {
                    // Anonpack
                    msg = await CryptoUtils.PackAsync(wallet, routingKey, new ForwardMessage { Message = msg.GetUTF8String(), To = previousKey });
                    previousKey = routingKey;
                }
            }

            return msg;
        }
        
        private async Task<MessageContext> UnpackAsync(Wallet wallet, MessageContext message, ConnectionRecord connection)
        {
            UnpackResult unpacked;

            try
            {
                unpacked = await CryptoUtils.UnpackAsync(wallet, message.Payload);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to un-pack message", e);
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Failed to un-pack message", e);
            }

            message = new MessageContext(unpacked.Message, false, connection);

            return message;
        }

        /// <inheritdoc />
        public virtual async Task<MessageContext> SendAsync(Wallet wallet, AgentMessage message, ConnectionRecord connection, string recipientKey = null, bool requestResponse = false)
        {
            recipientKey = recipientKey
                                ?? connection.TheirVk
                                ?? throw new AgentFrameworkException(
                                    ErrorCode.A2AMessageTransmissionError, "Cannot find encryption key");

            var routingKeys = connection.Endpoint?.Verkey != null ? new[] { connection.Endpoint.Verkey } : new string[0];

            if (connection.Endpoint?.Uri == null)
                throw new AgentFrameworkException(ErrorCode.A2AMessageTransmissionError, "Cannot send to connection that does not have endpoint information specified");

            var response = await SendAsync(wallet, message, recipientKey, connection.Endpoint.Uri, routingKeys, connection.MyVk, requestResponse);

            if (response?.Packed != null)
            {
                response = await UnpackAsync(wallet, response, connection);
            }

            return response;
        }

        /// <inheritdoc />
        public virtual async Task<MessageContext> SendAsync(Wallet wallet, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null, bool requestResponse = false)
        {
            Logger.LogInformation(LoggingEvents.SendMessage, "Recipient {0} Endpoint {1}", recipientKey,
                endpointUri);

            if (string.IsNullOrEmpty(message.Id))
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "@id field on message must be populated");

            if (string.IsNullOrEmpty(message.Type))
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "@type field on message must be populated");

            if (string.IsNullOrEmpty(endpointUri))
                throw new ArgumentNullException(nameof(endpointUri));

            var uri = new Uri(endpointUri);

            var dispatcher = GetDispatcher(uri.Scheme);

            if (dispatcher == null)
                throw new AgentFrameworkException(ErrorCode.A2AMessageTransmissionError, $"No registered dispatcher for transport scheme : {uri.Scheme}");

            if (requestResponse)
                message.AddReturnRouting();

            var wireMsg = await PrepareAsync(wallet, message, recipientKey, routingKeys, senderKey);

            return await dispatcher.DispatchAsync(uri, new MessageContext(wireMsg, true));
        }

        private IMessageDispatcher GetDispatcher(string scheme) => MessageDispatchers.FirstOrDefault(_ => _.TransportSchemes.Contains(scheme));
    }
}