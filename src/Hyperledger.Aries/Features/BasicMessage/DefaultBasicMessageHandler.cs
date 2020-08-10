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
        private readonly IWalletRecordService _recordService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBasicMessageHandler"/> class.
        /// </summary>
        /// <param name="recordService">The record service.</param>
        public DefaultBasicMessageHandler(IWalletRecordService recordService)
        {
            _recordService = recordService;
        }

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
            var record = new BasicMessageRecord
            {
                Id = Guid.NewGuid().ToString(),
                ConnectionId = messageContext.Connection.Id,
                Text = message.Content,
                SentTime = DateTime.TryParse(message.SentTime, out var dateTime) ? dateTime : DateTime.UtcNow,
                Direction = MessageDirection.Incoming
            };
            await _recordService.AddAsync(agentContext.Wallet, record);
            messageContext.ContextRecord = record;

            return null;
        }
    }
}
