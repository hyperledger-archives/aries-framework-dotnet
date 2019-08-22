using System;
using System.Collections.Generic;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using System.Threading.Tasks;
using AgentFramework.Core.Decorators.Threading;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Events;

namespace WebAgent.Messages
{
    public class TrustPingMessageHandler : IMessageHandler
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// The message service.
        /// </summary>
        private readonly IMessageService _messageService;

        public TrustPingMessageHandler(IEventAggregator eventAggregator, IMessageService messageService)
        {
            _eventAggregator = eventAggregator;
            _messageService = messageService;
        }

        /// <summary>
        /// Gets the supported message types.
        /// </summary>
        /// <value>
        /// The supported message types.
        /// </value>
        public IEnumerable<MessageType> SupportedMessageTypes => new[]
        {
            MessageType.FromUri(CustomMessageTypes.TrustPingMessageType),
            MessageType.FromUri(CustomMessageTypes.TrustPingResponseMessageType)
        };

        /// <summary>
        /// Processes the agent message
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="messageContext">The agent message context.</param>
        public async Task<AgentMessage> ProcessAsync(IAgentContext agentContext, MessageContext messageContext)
        {
            await Task.Yield();

            switch (messageContext.GetMessageType())
            {
                case CustomMessageTypes.TrustPingMessageType:
                    {
                        var pingMessage = messageContext.GetMessage<TrustPingMessage>();

                        if (pingMessage.ResponseRequested)
                        {
                            return pingMessage.CreateThreadedReply<TrustPingResponseMessage>();
                        }
                        break;
                    }
                case CustomMessageTypes.TrustPingResponseMessageType:
                    {
                        _eventAggregator.Publish(new ServiceMessageProcessingEvent
                        {
                            MessageType = CustomMessageTypes.TrustPingResponseMessageType
                        });
                        break;
                    }
            }
            return null;
        }
    }
}