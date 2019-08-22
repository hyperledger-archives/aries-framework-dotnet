using AgentFramework.Core.Messages.Discovery;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Discovery Service.
    /// </summary>
    public interface IDiscoveryService
    {
        /// <summary>
        /// Creates a discovery query message.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="query">Query for message.</param>
        /// <returns>A discovery query message.</returns>
        DiscoveryQueryMessage CreateQuery(IAgentContext agentContext, string query);
        
        /// <summary>
        /// Creates a discovery disclose message from a query message.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="message">Query message.</param>
        /// <returns>A discovery disclose message.</returns>
        DiscoveryDiscloseMessage CreateQueryResponse(IAgentContext agentContext, DiscoveryQueryMessage message);
    }
}