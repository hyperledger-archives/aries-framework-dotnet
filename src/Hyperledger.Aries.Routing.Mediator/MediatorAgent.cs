using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class DefaultMediatorAgent : AgentBase
    {
        public DefaultMediatorAgent(IServiceProvider provider) : base(provider)
        {
        }

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddDiscoveryHandler();
            AddTrustPingHandler();
            AddHandler<MediatorForwardHandler>();
            AddHandler<DefaultStoreBackupHandler>();
            AddHandler<RetrieveBackupHandler>();
            AddHandler<RoutingInboxHandler>();
        }
    }
}
