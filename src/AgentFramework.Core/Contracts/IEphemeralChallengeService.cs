using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Messages.EphemeralChallenge;
using AgentFramework.Core.Models.EphemeralChallenge;
using AgentFramework.Core.Models.Proofs;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Ephemeral challenge service.
    /// </summary>
    public interface IEphemeralChallengeService
    {
        /// <summary>
        /// Accepts an ephemeral challenge.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="message">The challenge message to accept.</param>
        /// <param name="credentials">The requested credentials.</param>
        /// <returns>The ephemeral challenge response message.</returns>
        Task<EphemeralChallengeResponseMessage> CreateProofChallengeResponseAsync(IAgentContext agentContext, EphemeralChallengeMessage message, RequestedCredentials credentials);

        /// <summary>
        /// Creates an ephemeral challenge.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="challengeConfigId">Challenge configuration identifier.</param>
        /// <returns>Challenge configuration identifier.</returns>
        Task<(EphemeralChallengeMessage, EphemeralChallengeRecord)> CreateChallengeAsync(IAgentContext agentContext, string challengeConfigId);

        /// <summary>
        /// Creates a challenge configuration record.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="config">The challenge configuration.</param>
        /// <returns>The challenge configuration identifier.</returns>
        Task<string> CreateChallengeConfigAsync(IAgentContext agentContext, EphemeralChallengeConfiguration config);

        /// <summary>
        /// Gets a challenge by identifier.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="challengeId">The challenge id.</param>
        /// <returns>The challenge record.</returns>
        Task<EphemeralChallengeRecord> GetChallengeAsync(IAgentContext agentContext, string challengeId);

        /// <summary>
        /// Gets a challenge configuration record.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="configId">The configuration id.</param>
        /// <returns>The challenge configuration record.</returns>
        Task<EphemeralChallengeConfigRecord> GetChallengeConfigAsync(IAgentContext agentContext, string configId);

        /// <summary>
        /// Get the challenges current state.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="challengeId">The challenge id.</param>
        /// <param name="deleteIfResolved">If the challenge is resolved delete it from persistance.</param>
        /// <returns>The current challenge state.</returns>
        Task<ChallengeState> GetChallengeStateAsync(IAgentContext agentContext, string challengeId,
            bool deleteIfResolved = true);

        /// <summary>
        /// Lists the challenge configurations.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="query">The query to limit the result sets.</param>
        /// <param name="count">The count of credentials.</param>
        /// <returns>The challenge configuration records.</returns>
        Task<List<EphemeralChallengeConfigRecord>> ListChallengeConfigsAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100);

        /// <summary>
        /// Lists the challenges.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="query">The query to limit the result sets.</param>
        /// <param name="count">The count of credentials.</param>
        /// <returns>The challenge records.</returns>
        Task<List<EphemeralChallengeRecord>> ListChallengesAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100);

        /// <summary>
        /// Processes an ephemeral challenge response.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="challengeResponse">The ephemeral challenge response.</param>
        /// <returns>The identifier of the record associated to the message.</returns>
        Task<string> ProcessChallengeResponseAsync(IAgentContext agentContext, EphemeralChallengeResponseMessage challengeResponse);
    }
}