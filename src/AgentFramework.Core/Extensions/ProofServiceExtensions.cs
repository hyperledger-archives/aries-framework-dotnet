using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Utils;

namespace AgentFramework.Core.Extensions
{
    /// <summary>
    /// A collection of convenience methods for the <see cref="IProofService"/> class.
    /// </summary>
    public static class ProofServiceExtensions
    {
        /// <summary>Retrieves a list of proof request records.</summary>
        /// <param name="proofService">The proof service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static Task<List<ProofRecord>> ListRequestedAsync(this IProofService proofService,
            IAgentContext context, int count = 100)
            => proofService.ListAsync(context,
                SearchQuery.Equal(nameof(ProofRecord.State), ProofState.Requested.ToString("G")), count);

        /// <summary>Retrieves a list of accepted proof records.</summary>
        /// <param name="proofService">The proof service.</param>
        /// <param name="context">The context.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static Task<List<ProofRecord>> ListAcceptedAsync(this IProofService proofService,
            IAgentContext context, int count = 100)
            => proofService.ListAsync(context,
                SearchQuery.Equal(nameof(ProofRecord.State), ProofState.Accepted.ToString("G")), count);

        /// <summary>Retrieves a proof record by its thread id.</summary>
        /// <param name="proofService">Proof service.</param>
        /// <param name="context">The context.</param>
        /// <param name="threadId">The thread id.</param>
        /// <returns>The proof record.</returns>
        public static async Task<ProofRecord> GetByThreadIdAsync(
            this IProofService proofService, IAgentContext context, string threadId)
        {
            var search = await proofService.ListAsync(context, SearchQuery.Equal(TagConstants.LastThreadId, threadId), 1);

            if (search.Count == 0)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, $"Proof record not found by thread id : {threadId}");

            if (search.Count > 1)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState, $"Multiple proof records found by thread id : {threadId}");

            return search.Single();
        }
    }
}
