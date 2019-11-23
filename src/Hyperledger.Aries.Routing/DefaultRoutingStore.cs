using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Routing
{
    public class DefaultRoutingStore : IRoutingStore
    {
        private readonly IWalletRecordService recordService;
        private readonly IAgentProvider agentProvider;

        public DefaultRoutingStore(
            IWalletRecordService recordService,
            IAgentProvider agentProvider)
        {
            this.recordService = recordService;
            this.agentProvider = agentProvider;
        }

        public async Task AddRouteAsync(string destinationRoute, string inboxId)
        {
            var agentContext = await agentProvider.GetContextAsync();
            var routeRecord = new RouteRecord
            {
                Id = destinationRoute,
                InboxId = inboxId
            };
            await recordService.AddAsync(agentContext.Wallet, routeRecord);
        }

        public async Task<string> FindRouteAsync(string destinationRoute)
        {
            var agentContext = await agentProvider.GetContextAsync();
            var routeRecord = await recordService.GetAsync<RouteRecord>(agentContext.Wallet, destinationRoute);

            return routeRecord.InboxId;
        }
    }
}
