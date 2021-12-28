using Hyperledger.Aries.Agents;
using System;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Features.ProblemReport
{
    class DefaultProblemReportService : IProblemReportService
    {
        public Task<ProblemReportMessage> CreateProblemReport(IAgentContext agentContext, string comment)
        {
            // Add validation like in Aries.NET style
            var problemMessage = new ProblemReportMessage
            {
                Id = Guid.NewGuid().ToString(),
                Comment = comment,
                Type = MessageTypesHttps.ProblemReportMessageType,
                NoticedAt = DateTime.UtcNow
            };
            return Task.FromResult(problemMessage);
        }
    }
}
