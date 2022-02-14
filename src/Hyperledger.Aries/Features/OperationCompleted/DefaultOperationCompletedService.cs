using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.OperationCompleted.Messages;
using Hyperledger.Aries.Features.OperationCompleted.Models;
using Hyperledger.Aries.Storage;
using System;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.OperationCompleted
{
    public class DefaultOperationCompletedService : IOperationCompletedService
    {
        private readonly IWalletRecordService _recordService;

        public DefaultOperationCompletedService(IWalletRecordService recordService)
        {
            _recordService = recordService;
        }

        public Task<OperationCompletedMessage> CreateOperationCompletedMessage(string comment)
        {
            var message = new OperationCompletedMessage
            {
                Comment = comment
            };
            return Task.FromResult(message);
        }

        public async Task<OperationCompletedRecord> ProcessOperationCompletedMessage(IAgentContext agentContext, string comment, string connectionId)
        {
            var record = new OperationCompletedRecord
            {
                Id = Guid.NewGuid().ToString(),
                ConnectionId = connectionId,
                Comment = comment
            };
            await _recordService.AddAsync(agentContext.Wallet, record);

            return record;
        }
    }
}
