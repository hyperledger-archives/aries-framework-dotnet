using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.OperationCompleted.Messages;
using Hyperledger.Aries.Features.OperationCompleted.Models;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.OperationCompleted
{
    public interface IOperationCompletedService
    {
        Task<OperationCompletedMessage> CreateOperationCompletedMessage(string comment);
        Task<OperationCompletedRecord> ProcessOperationCompletedMessage(IAgentContext agentContext, string comment, string connectionId);
    }
}
