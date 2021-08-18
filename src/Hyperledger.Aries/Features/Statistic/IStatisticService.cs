using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.Statistic.Messages;
using Hyperledger.Aries.Features.Statistic.Models;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.Statistic
{
    public interface IStatisticService
    {
        Task<PresentProofStatisticMessage> CreatePresentProof(IAgentContext agentContext, PresentProofRequest presentProof, string connectionId);
        Task<PresentProofRecord> ProcessPresentProof(IAgentContext context, PresentProofStatisticMessage message, ConnectionRecord connection);
    }
}
