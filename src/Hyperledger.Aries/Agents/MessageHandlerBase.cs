using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// A convenience base class for implementing strong type handlers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IMessageHandler" />
    public abstract class MessageHandlerBase<T> : IMessageHandler
        where T : AgentMessage, new()
    {
        private readonly string _supportedMessageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerBase{T}"/> class.
        /// </summary>
        protected MessageHandlerBase()
        {
            _supportedMessageType = new T().Type;
        }

        /// <inheritdoc />
        public virtual IEnumerable<MessageType> SupportedMessageTypes => new[] { new MessageType(_supportedMessageType) };

        /// <summary>
        /// Processes the incoming <see cref="AgentMessage"/>
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="agentContext">The message agentContext.</param>
        /// <param name="messageContext">The message context.</param>
        /// <returns></returns>
        protected abstract Task<AgentMessage> ProcessAsync(T message, IAgentContext agentContext, UnpackedMessageContext messageContext);

        /// <inheritdoc />
        public Task<AgentMessage> ProcessAsync(IAgentContext agentContext, UnpackedMessageContext messageContext) =>
            ProcessAsync(messageContext.GetMessage<T>(), agentContext, messageContext);
    }
}
