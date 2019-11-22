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
            AddHandler<MediatorForwardHandler>();
            AddHandler<RoutingInboxHandler>();
        }
    }
}
