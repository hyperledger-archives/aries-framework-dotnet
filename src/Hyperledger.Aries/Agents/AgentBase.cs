using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.BasicMessage;
using Hyperledger.Aries.Features.Discovery;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Features.RevocationNotification;
using Hyperledger.Aries.Features.Routing;
using Hyperledger.Aries.Features.TrustPing;
using Hyperledger.Aries.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Base agent implementation
    /// </summary>
    public abstract class AgentBase : IAgent
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public IServiceProvider Provider { get; }

        /// <summary>Gets the connection service.</summary>
        /// <value>The connection service.</value>
        protected IConnectionService ConnectionService { get; }

        /// <summary>Gets the message service.</summary>
        /// <value>The message service.</value>
        protected IMessageService MessageService { get; }

        /// <summary>Gets the logger.</summary>
        /// <value>The logger.</value>
        protected ILogger<AgentBase> Logger { get; }

        /// <summary>
        /// Gets the handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public IList<IMessageHandler> Handlers { get; }

        /// <summary>
        /// Gets a collecrion of registered agent middlewares
        /// </summary>
        protected IEnumerable<IAgentMiddleware> Middlewares { get; }

        /// <summary>Initializes a new instance of the <see cref="AgentBase"/> class.</summary>
        protected AgentBase(IServiceProvider provider)
        {
            Provider = provider;
            ConnectionService = provider.GetRequiredService<IConnectionService>();
            MessageService = provider.GetRequiredService<IMessageService>();
            Logger = provider.GetRequiredService<ILogger<AgentBase>>();
            Handlers = new List<IMessageHandler>();
            Middlewares = provider.GetServices<IAgentMiddleware>();
        }

        /// <summary>Adds a handler for supporting default out-of-band flows.</summary>
        protected void AddOutOfBandHandler() => Handlers.Add(Provider.GetRequiredService<DefaultOutOfBandHandler>());
        
        /// <summary>Adds a handler for supporting default connection flow.</summary>
        protected void AddConnectionHandler() => Handlers.Add(Provider.GetRequiredService<DefaultConnectionHandler>());

        /// <summary>Adds a handler for supporting default credential flow.</summary>
        protected void AddCredentialHandler() => Handlers.Add(Provider.GetRequiredService<DefaultCredentialHandler>());

        /// <summary>Adds a handler for supporting default did exchange flow.</summary>
        protected void AddDidExchangeHandler() => Handlers.Add(Provider.GetRequiredService<DefaultDidExchangeHandler>());

        /// <summary>Adds the handler for supporting default proof flow.</summary>
        protected void AddTrustPingHandler() => Handlers.Add(Provider.GetRequiredService<DefaultTrustPingMessageHandler>());

        /// <summary>Adds the handler for supporting default proof flow.</summary>
        protected void AddProofHandler() => Handlers.Add(Provider.GetRequiredService<DefaultProofHandler>());

        /// <summary>Adds the default handler for supporting revocation notifications.</summary>
        protected void AddRevocationNotificationHandler() =>
            Handlers.Add(Provider.GetRequiredService<DefaultRevocationNotificationHandler>());

        /// <summary>Adds a default forwarding handler.</summary>
        protected void AddForwardHandler() => Handlers.Add(Provider.GetRequiredService<DefaultForwardHandler>());

        /// <summary>Adds a default basic message handler.</summary>
        protected void AddBasicMessageHandler() => Handlers.Add(Provider.GetRequiredService<DefaultBasicMessageHandler>());

        /// <summary>Adds a default discovery handler.</summary>
        protected void AddDiscoveryHandler() => Handlers.Add(Provider.GetRequiredService<DefaultDiscoveryHandler>());

        /// <summary>Adds a custom the handler using dependency injection.</summary>
        /// <typeparam name="T"></typeparam>
        protected void AddHandler<T>() where T : IMessageHandler => Handlers.Add(Provider.GetRequiredService<T>());

        /// <summary>Adds an instance of a custom handler.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        protected void AddHandler<T>(T instance) where T : IMessageHandler => Handlers.Add(instance);

        /// <summary>
        /// Invoke the handler pipeline and process the passed message.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="messageContext">The message context.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Expected inner message to be of type 'ForwardMessage'</exception>
        /// <exception cref="AriesFrameworkException">Couldn't locate a message handler for type {messageType}</exception>
        /// TODO should receive a message context and return a message context.
        public async Task<MessageContext> ProcessAsync(IAgentContext context, MessageContext messageContext)
        {
            EnsureConfigured();

            if (context is DefaultAgentContext agentContext)
            {
                agentContext.AddNext(messageContext);
                agentContext.Agent = this;

                MessageContext outgoingMessageContext = null;
                while (agentContext.TryGetNext(out var message) && outgoingMessageContext == null)
                {
                    outgoingMessageContext = await ProcessMessage(agentContext, message);
                }
                return outgoingMessageContext;
            }
            throw new Exception("Unsupported agent context. When using custom context, please inherit from 'DefaultAgentContext'");
        }

        private async Task<MessageContext> ProcessMessage(IAgentContext agentContext, MessageContext messageContext)
        {
            UnpackResult unpacked = null;
            UnpackedMessageContext inboundMessageContext = null;
            if (messageContext is PackedMessageContext packedMessageContext)
            {
                (inboundMessageContext, unpacked) = await UnpackAsync(agentContext, packedMessageContext);
                Logger.LogInformation($"Agent Message Received : {inboundMessageContext.ToJson()}");
            }

            if (Handlers.Where(handler => handler != null).FirstOrDefault(
                    handler => handler.SupportedMessageTypes.Any(
                        type => type == inboundMessageContext.GetMessageType())) is
                IMessageHandler messageHandler)
            {
                Logger.LogDebug("Processing message type {MessageType}, {MessageData}",
                    inboundMessageContext.GetMessageType(),
                    inboundMessageContext.Payload.GetUTF8String());

                // Process message in handler
                AgentMessage response;
                try
                {
                    response = await messageHandler.ProcessAsync(agentContext, inboundMessageContext);
                }
                catch (AriesFrameworkException e)
                {
                    throw new AriesFrameworkException(e.ErrorCode,e.Message,inboundMessageContext.ContextRecord,inboundMessageContext.Connection);
                }

                // Process message with any registered middlewares
                foreach (var middleware in Middlewares)
                {
                    await middleware.OnMessageAsync(agentContext, inboundMessageContext);
                }

                if (response != null)
                {
                    if (inboundMessageContext.ReturnRoutingRequested())
                    {
                        var result = inboundMessageContext.Connection != null
                            ? await CryptoUtils.PackAsync(agentContext.Wallet, inboundMessageContext.Connection.TheirVk, response.ToByteArray())
                            : await CryptoUtils.PackAsync(agentContext.Wallet, unpacked.SenderVerkey, response.ToByteArray());
                        return new PackedMessageContext(result);
                    }
                    if (inboundMessageContext.Connection != null)
                    {
                        await MessageService.SendAsync(agentContext, response, inboundMessageContext.Connection);
                    }
                    else
                    {
                        Logger.LogWarning("Return response available, but connection was not found or was in invalid state");
                    }
                }
                return null;
            }

            throw new AriesFrameworkException(ErrorCode.InvalidMessage,
                $"Couldn't locate a message handler for type {inboundMessageContext.GetMessageType()}");
        }

        private async Task<(UnpackedMessageContext, UnpackResult)> UnpackAsync(IAgentContext agentContext, PackedMessageContext message)
        {
            UnpackResult unpacked;

            try
            {
                unpacked = await CryptoUtils.UnpackAsync(agentContext.Wallet, message.Payload);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to un-pack message", e);
                throw new AriesFrameworkException(ErrorCode.InvalidMessage, "Failed to un-pack message", e);
            }

            UnpackedMessageContext result = null;
            if (unpacked.SenderVerkey != null && message.Connection == null)
            {
                try
                {
                    if (await ConnectionService.ResolveByMyKeyAsync(agentContext, unpacked.RecipientVerkey) is ConnectionRecord connection)
                    {
                        result = new UnpackedMessageContext(unpacked.Message, connection);
                    }
                    else
                    {
                        result = new UnpackedMessageContext(unpacked.Message, unpacked.SenderVerkey);
                    }
                }
                catch (AriesFrameworkException ex) when (ex.ErrorCode == ErrorCode.RecordNotFound)
                {
                    // OK if not resolved. Example: authpacked forward message in routing agent.
                    // Downstream consumers should throw if Connection is required
                }
            }
            else
            {
                if (message.Connection != null)
                {
                    result = new UnpackedMessageContext(unpacked.Message, message.Connection);
                }
                else
                {
                    result = new UnpackedMessageContext(unpacked.Message, unpacked.SenderVerkey);
                }

            }

            return (result, unpacked);
        }

        private void EnsureConfigured()
        {
            if (Handlers == null || !Handlers.Any())
                ConfigureHandlers();
        }

        /// <summary>Configures the handlers.</summary>
        protected abstract void ConfigureHandlers();
    }
}
