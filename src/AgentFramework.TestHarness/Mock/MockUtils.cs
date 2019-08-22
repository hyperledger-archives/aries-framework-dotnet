using System;
using System.Net.Http;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.TestHarness.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace AgentFramework.TestHarness.Mock
{
    public class MockUtils
    {
        public static async Task<MockAgent> CreateAsync(string agentName, WalletConfiguration configuration, WalletCredentials credentials, MockAgentHttpHandler handler, string issuerSeed = null)
        {
            var services = new ServiceCollection();

            services.AddAgentFramework();
            services.AddDefaultMessageHandlers();
            services.AddLogging();
            services.AddSingleton<MockAgentMessageProcessor>();
            services.AddSingleton<IHttpClientFactory>(new InProcAgent.InProcFactory(handler));

            return await CreateAsync(agentName, configuration, credentials, services, issuerSeed);
        }

        public static async Task<MockAgent> CreateAsync(string agentName, WalletConfiguration configuration, WalletCredentials credentials, ServiceCollection services, string issuerSeed = null)
        {
            var provider = services.BuildServiceProvider();

            await provider.GetService<IProvisioningService>()
                .ProvisionAgentAsync(new IssuerProvisioningConfiguration { WalletConfiguration = configuration, WalletCredentials = credentials, EndpointUri = new Uri($"http://{agentName}"), IssuerSeed = issuerSeed });

            return new MockAgent(agentName, provider)
            {
                Context = new AgentContext
                {
                    Wallet = await provider.GetService<IWalletService>().GetWalletAsync(configuration, credentials),
                    Pool = new PoolAwaitable(PoolUtils.GetPoolAsync),
                    SupportedMessages = AgentUtils.GetDefaultMessageTypes()
                },
                ServiceProvider = provider
            };
        }

        public static async Task Dispose(MockAgent agent)
        {
            agent.Context.Wallet.Dispose();
            await agent.Context.Wallet.CloseAsync();
        }
    }
}
