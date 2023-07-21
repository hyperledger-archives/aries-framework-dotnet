using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Decorators.Transport;
using Hyperledger.Aries.Utils;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;

namespace Hyperledger.Aries.Agents
{
    /// <inheritdoc />
    public class DefaultMessageService : IMessageService
    {
        /// <summary>The encrypted envelope message MIME type</summary>
        public const string EncryptedEnvelopeMessageMimeType = "application/didcomm-envelope-enc";

        /// <summary>The agent wire message MIME type</summary>
        public const string AgentWireMessageMimeType = "application/ssi-agent-wire";
        
        /// <summary>The agent wire message MIME type</summary>
        public const string JsonMessageMimeType = "application/json";

        public static readonly IEnumerable<string> SupportedMimeTypes = new List<string>
        {
            EncryptedEnvelopeMessageMimeType,
            AgentWireMessageMimeType,
            JsonMessageMimeType
        };

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

        private async Task<UnpackedMessageContext> UnpackAsync(Wallet wallet, PackedMessageContext message, string senderKey)
        {
            UnpackResult unpacked;

            try
            {
                unpacked = await CryptoUtils.UnpackAsync(wallet, message.Payload);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to un-pack message", e);
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Failed to un-pack message", e);
            }
            return new UnpackedMessageContext(unpacked.Message, senderKey);
        }

        /// <inheritdoc />
        public virtual async Task SendAsync(IAgentContext agentContext, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null)
        {
            Logger.LogInformation(LoggingEvents.SendMessage, "Recipient {0} Endpoint {1}", recipientKey,
                endpointUri);

            if (string.IsNullOrEmpty(message.Id))
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "@id field on message must be populated");

            if (string.IsNullOrEmpty(message.Type))
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "@type field on message must be populated");

            if (string.IsNullOrEmpty(endpointUri))
                throw new ArgumentNullException(nameof(endpointUri));

            var uri = new Uri(endpointUri);

            var dispatcher = GetDispatcher(uri.Scheme);

            if (dispatcher == null)
                throw new AriesFrameworkException(ErrorCode.A2AMessageTransmissionError, $"No registered dispatcher for transport scheme : {uri.Scheme}");

            var wireMsg = await CryptoUtils.PrepareAsync(agentContext, message, recipientKey, routingKeys, senderKey);

            await dispatcher.DispatchAsync(uri, new PackedMessageContext(wireMsg));
        }

        /// <inheritdoc />
        public async Task<MessageContext> SendReceiveAsync(IAgentContext agentContext, AgentMessage message, string recipientKey,
            string endpointUri, string[] routingKeys = null, string senderKey = null, ReturnRouteTypes returnType = ReturnRouteTypes.all)
        {
            Logger.LogInformation(LoggingEvents.SendMessage, "Recipient {0} Endpoint {1}", recipientKey,
                endpointUri);

            if (string.IsNullOrEmpty(message.Id))
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "@id field on message must be populated");

            if (string.IsNullOrEmpty(message.Type))
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "@type field on message must be populated");

            if (string.IsNullOrEmpty(endpointUri))
                throw new ArgumentNullException(nameof(endpointUri));

            var uri = new Uri(endpointUri);

            var dispatcher = GetDispatcher(uri.Scheme);

            if (dispatcher == null)
                throw new AriesFrameworkException(ErrorCode.A2AMessageTransmissionError, $"No registered dispatcher for transport scheme : {uri.Scheme}");

            message.AddReturnRouting(returnType);
            var wireMsg = await CryptoUtils.PrepareAsync(agentContext, message, recipientKey, routingKeys, senderKey);

            var response = await dispatcher.DispatchAsync(uri, new PackedMessageContext(wireMsg));
            if (response is PackedMessageContext responseContext)
            {
                return await UnpackAsync(agentContext.Wallet, responseContext, senderKey);
            }
            throw new InvalidOperationException("Invalid or empty response");
        }

        private IMessageDispatcher GetDispatcher(string scheme) => MessageDispatchers.FirstOrDefault(_ => _.TransportSchemes.Contains(scheme));
    }
}
