using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;

namespace AgentFramework.TestHarness.Mock
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
