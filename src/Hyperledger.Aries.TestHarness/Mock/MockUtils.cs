﻿using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Storage;
using Hyperledger.TestHarness.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperledger.TestHarness.Mock
{
    public class MockUtils
    {
        public static async Task<MockAgent> CreateAsync(string agentName, WalletConfiguration configuration, WalletCredentials credentials, MockAgentHttpHandler handler, string issuerSeed = null)
        {
            var services = new ServiceCollection();

            services.AddAriesFramework();
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
                .ProvisionAgentAsync(new AgentOptions
                {
                    EndpointUri = $"http://{agentName}",
                    IssuerKeySeed = issuerSeed,
                    WalletConfiguration = configuration,
                    WalletCredentials = credentials,
                });

            return new MockAgent(agentName, provider)
            {
                Context = new DefaultAgentContext
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
