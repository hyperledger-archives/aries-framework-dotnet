using Hyperledger.Aries.Agents;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.ProblemReport
{
    public interface IProblemReportService
    {
        Task<ProblemReportMessage> CreateProblemReport(IAgentContext agentContext, string comment);
    }
}
