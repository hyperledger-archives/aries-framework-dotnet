using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Messages;

namespace WebAgent.Protocols.BasicMessage
{
    public class BasicMessageHandler : MessageHandlerBase<BasicMessage>
    {
        private readonly IWalletRecordService _recordService;

        public BasicMessageHandler(IWalletRecordService recordService)
        {
            _recordService = recordService;
        }

        protected override async Task<AgentMessage> ProcessAsync(BasicMessage message, IAgentContext agentContext, MessageContext messageContext)
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