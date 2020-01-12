using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;

namespace WebAgent.Protocols.BasicMessage
{
    public class BasicMessageHandler : MessageHandlerBase<BasicMessage>
    {
        private readonly IWalletRecordService _recordService;

        public BasicMessageHandler(IWalletRecordService recordService)
        {
            _recordService = recordService;
        }

        protected override async Task<AgentMessage> ProcessAsync(BasicMessage message, IAgentContext agentContext, UnpackedMessageContext messageContext)
        {
            Console.WriteLine($"Processing message by {messageContext.Connection.Id}");

            await _recordService.AddAsync(agentContext.Wallet, new BasicMessageRecord
            {
                Id = Guid.NewGuid().ToString(),
                ConnectionId = messageContext.Connection.Id,
                Text = message.Content,
                SentTime = DateTime.TryParse(message.SentTime, out var dateTime) ? dateTime : DateTime.UtcNow,
                Direction = MessageDirection.Incoming
            });

            return null;
        }
    }
}