using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Agents.Edge;

namespace Hyperledger.Aries.Routing
{
    public interface IEdgeClientService
    {
        Task<AgentPublicConfiguration> DiscoverConfigurationAsync(string agentEndpoint);

        Task CreateInboxAsync(IAgentContext agentContext, Dictionary<string, string> metadata = null);

        Task AddRouteAsync(IAgentContext agentContext, string routeDestination);

        Task AddDeviceAsync(IAgentContext agentContext, AddDeviceInfoMessage message);

        Task FetchInboxAsync(IAgentContext agentContext);
        
        Task CreateBackup(IAgentContext context, string key);
        
        Task RetrieveBackup(IAgentContext context, string id);
    }
}