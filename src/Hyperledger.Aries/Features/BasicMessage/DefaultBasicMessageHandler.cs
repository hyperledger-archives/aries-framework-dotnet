using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.BasicMessage
{
    /// <summary>
    /// Default basic message handler
    /// </summary>
    /// <seealso cref="MessageHandlerBase{BasicMessage}" />
    public class DefaultBasicMessageHandler : MessageHandlerBase<BasicMessage>
    {
        private readonly IBasicMessageService _basicMessageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBasicMessageHandler"/> class.
        /// </summary>
        /// <param name="basicMessageService">The basic message service.</param>
        public DefaultBasicMessageHandler(
            IBasicMessageService basicMessageService)
        {
            _basicMessageService = basicMessageService;
        }

        /// <inheritdoc />
        public override IEnumerable<MessageType> SupportedMessageTypes => new MessageType[] { MessageTypes.BasicMessageType, MessageTypesHttps.BasicMessageType };

        /// <summary>
        /// Processes the incoming <see cref="AgentMessage" />
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="agentContext">The message agentContext.</param>
        /// <param name="messageContext">The message context.</param>
        /// <returns></returns>
        protected override async Task<AgentMessage> ProcessAsync(BasicMessage message, IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            var record = await _basicMessageService.ProcessIncomingBasicMessageAsync(agentContext, messageContext.Connection.Id, message);
            messageContext.ContextRecord = record;
            return null;
        }
    }
}
