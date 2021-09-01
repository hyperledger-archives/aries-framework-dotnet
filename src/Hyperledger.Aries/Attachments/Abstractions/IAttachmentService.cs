using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Attachments.Records;
using Hyperledger.Aries.Features.IssueCredential;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Attachments.Abstractions
{
    public interface IAttachmentService
    {
        /// <summary>
        /// Gets attachment record for the given identifier.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="id">The credential identifier.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The stored credential record</returns>
        Task<AttachmentRecord> GetAsync(IAgentContext agentContext, string id);

        /// <summary>
        /// Create attachment.
        /// </summary>
        Task<string> Create(IAgentContext context, AgentMessage message, string recordId, string nickname);

        Task<AttachmentRecord> GetByRecordIdAsync(IAgentContext agentContext, string id);
    }
}
