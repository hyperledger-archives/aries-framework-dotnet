using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.TestHarness.Mock
{
    public class MockAgentMessageProcessor : AgentBase
    {
        public MockAgentMessageProcessor(
            IServiceProvider provider) : base(provider)
        {
        }

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddForwardHandler();
        }

        internal Task HandleAsync(MessageContext msg, IAgentContext context) => ProcessAsync(context, msg);
    }
}
