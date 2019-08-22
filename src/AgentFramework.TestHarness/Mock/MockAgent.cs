using System;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.TestHarness.Mock
{
    public class MockAgent : AgentBase
    {
        public MockAgent(string name, IServiceProvider provider) : base(provider)
        {
            Name = name;
        }

        public string Name { get; }

        public IAgentContext Context { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public T GetService<T>() => ServiceProvider.GetRequiredService<T>();

        public Task<MessageResponse> HandleInboundAsync(MessageContext messageContext) => ProcessAsync(Context, messageContext);

        public async Task Dispose() => await Context.Wallet.CloseAsync();

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddForwardHandler();
            AddCredentialHandler();
            AddProofHandler();
            AddDiscoveryHandler();
        }
    }
}
