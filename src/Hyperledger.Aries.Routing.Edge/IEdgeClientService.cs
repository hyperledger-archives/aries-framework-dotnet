using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;

namespace Hyperledger.Aries.Routing
{
    /// <summary>
    /// Edge Client Service
    /// </summary>
    public interface IEdgeClientService
    {
        /// <summary>
        /// Discovers the mediator configuration.
        /// </summary>
        /// <param name="agentEndpoint">The agent endpoint.</param>
        /// <returns></returns>
        Task<AgentPublicConfiguration> DiscoverConfigurationAsync(string agentEndpoint);

        /// <summary>
        /// Create new inbox for delivery with the mediator agent.
        /// </summary>
        /// <remarks>
        /// This method is called internally during the provisioning process and doesn't need to be
        /// invoked in normal workflows.
        /// </remarks>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="metadata">The metadata.</param>
        /// <returns></returns>
        Task CreateInboxAsync(IAgentContext agentContext, Dictionary<string, string> metadata = null);

        /// <summary>
        /// Associate a delivery route with the mediator agent.
        /// </summary>
        /// <remarks>
        /// This method is called internally during the provisioning process and doesn't need to be
        /// invoked in normal workflows.
        /// </remarks>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="routeDestination">The route destination.</param>
        /// <returns></returns>
        Task AddRouteAsync(IAgentContext agentContext, string routeDestination);

        /// <summary>
        /// Register a device with the mediator agent.
        /// </summary>
        /// <remarks>
        /// This can be used for registering devices for push notifications
        /// </remarks>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task AddDeviceAsync(IAgentContext agentContext, AddDeviceInfoMessage message);

        /// <summary>
        /// Fetch and process all messages from the mediator agent
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <returns></returns>
        Task FetchInboxAsync(IAgentContext agentContext);

        /// <summary>
        /// Creates a backup for the current edge agent.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        Task<string> CreateBackupAsync(IAgentContext context, string seed);

        /// <summary>
        /// Retrieves the latest available backup.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        Task<List<Attachment>> RetrieveBackupAsync(IAgentContext context, string seed, long offset = default);

        /// <summary>
        /// Retrieves a list of available backups
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task<List<long>> ListBackupsAsync(IAgentContext context);

        /// <summary>
        /// Restores the agent and wallet from backup. Removes the existing wallet and creates a new one with same
        /// configuration
        /// </summary>
        /// <param name="edgeContext">The edge context.</param>
        /// <param name="seed">The seed.</param>
        /// <param name="backupData">The backup(Attachments) data.</param>
        /// <returns></returns>
        [Obsolete("This method is obsolete.")]
        Task RestoreFromBackupAsync(IAgentContext edgeContext, string seed, List<Attachment> backupData);

        /// <summary>
        /// Restores the agent and wallet from backup. Removes the existing wallet and creates a new one with same
        /// configuration
        /// </summary>
        /// <param name="edgeContext">The edge context.</param>
        /// <param name="seed">The seed.</param>
        /// <returns></returns>
        Task RestoreFromBackupAsync(IAgentContext edgeContext, string seed);
    }
}