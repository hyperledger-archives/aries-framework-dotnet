using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperledger.TestHarness.Mock
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

        public Task<MessageContext> HandleInboundAsync(MessageContext messageContext) => ProcessAsync(Context, messageContext);

        public async Task Dispose() => await Context.Wallet.CloseAsync();

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddDidExchangeHandler();
            AddForwardHandler();
            AddCredentialHandler();
            AddProofHandler();
            AddDiscoveryHandler();
            AddRevocationNotificationHandler();
        }
    }
}
