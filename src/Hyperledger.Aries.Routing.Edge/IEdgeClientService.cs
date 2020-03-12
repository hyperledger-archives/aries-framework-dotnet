using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Agents.Edge;
using Hyperledger.Aries.Decorators.Attachments;

namespace Hyperledger.Aries.Routing
{
    public interface IEdgeClientService
    {
        Task<AgentPublicConfiguration> DiscoverConfigurationAsync(string agentEndpoint);

        Task CreateInboxAsync(IAgentContext agentContext, Dictionary<string, string> metadata = null);

        Task AddRouteAsync(IAgentContext agentContext, string routeDestination);

        Task AddDeviceAsync(IAgentContext agentContext, AddDeviceInfoMessage message);

        Task FetchInboxAsync(IAgentContext agentContext);

        /// <summary>
        /// Creates a backup for the current edge agent.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        Task<DateTimeOffset> CreateBackupAsync(IAgentContext context, string seed);

        /// <summary>
        /// Retrieves the latest available backup.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        Task<List<Attachment>> RetrieveBackupAsync(IAgentContext context, string seed, DateTimeOffset offset = default);

        /// <summary>
        /// Get a list of available backup dates
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        Task<List<DateTimeOffset>> ListBackupsAsync(IAgentContext context, string seed);

        /// <summary>
        /// Restores the agent and wallet from backup.
        /// </summary>
        /// <param name="edgeContext">The edge context.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="backupData">The backup data.</param>
        /// <param name="newWalletConfiguration">The configuration for the new wallet.</param>
        /// <param name="newKey">The key for the new wallet.</param>
        /// <returns></returns>
        Task RestoreFromBackupAsync(IAgentContext edgeContext, string seed, List<Attachment> backupData, string newWalletConfiguration, string newKey);
    }
}