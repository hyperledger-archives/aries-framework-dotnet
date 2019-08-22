using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Models.Connections;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.Payments.SovrinToken;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace AgentFramework.TestHarness.Mock
{
    public class InProcAgent : AgentBase, IAsyncLifetime
    {
        /// <inheritdoc />
        public InProcAgent(IHost host) 
            : base(host.Services.GetService<IServiceProvider>())
        {
            Host = host;
        }

        public IHost Host { get; }
        public IAgentContext Context { get; private set; }
        public IWalletRecordService Records => Host.Services.GetService<IWalletRecordService>();
        public IConnectionService Connections => Host.Services.GetService<IConnectionService>();
        public IMessageService Messages => Host.Services.GetService<IMessageService>();
        public IPaymentService Payments => Host.Services.GetService<IPaymentService>();

        internal Task<MessageResponse> HandleAsync(byte[] data) => 
            ProcessAsync(Context, new MessageContext(data, true));

        /// <inheritdoc />
        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddCredentialHandler();
            AddDiscoveryHandler();
            AddDiscoveryHandler();
            AddForwardHandler();
            AddProofHandler();
            AddEphemeralChallengeHandler();
            AddBasicMessageHandler();
        }

        #region Factory methods

        public static async Task<PairedAgents> CreatePairedAsync(bool establishConnection)
        {
            var handler1 = new InProcMessageHandler();
            var handler2 = new InProcMessageHandler();

            var agent1 = Create(handler1);
            var agent2 = Create(handler2);

            handler1.TargetAgent = agent2;
            handler2.TargetAgent = agent1;

            await agent1.InitializeAsync();
            await agent2.InitializeAsync();

            var result = new PairedAgents
            {
                Agent1 = agent1,
                Agent2 = agent2
            };

            if (establishConnection)
            {
                (result.Connection1, result.Connection2) = await ConnectAsync(agent1, agent2);
            }
            return result;
        }

        private static async Task<(ConnectionRecord agent1Connection, ConnectionRecord agent2Connection)> ConnectAsync(InProcAgent agent1, InProcAgent agent2)
        {
            var (invitation, agent1Connection) = await agent1.Provider.GetService<IConnectionService>().CreateInvitationAsync(agent1.Context, new InviteConfiguration { AutoAcceptConnection = true });

            var (request, agent2Connection) = await agent2.Provider.GetService<IConnectionService>().CreateRequestAsync(agent2.Context, invitation);
            _ = await agent2.Provider.GetService<IMessageService>().SendAsync(agent2.Context.Wallet, request, agent2Connection, invitation.RecipientKeys.First());

            agent1Connection = await agent1.Provider.GetService<IWalletRecordService>().GetAsync<ConnectionRecord>(agent1.Context.Wallet, agent1Connection.Id);
            agent2Connection = await agent2.Provider.GetService<IWalletRecordService>().GetAsync<ConnectionRecord>(agent2.Context.Wallet, agent2Connection.Id);

            return (agent1Connection, agent2Connection);
        }

        private static InProcAgent Create(HttpMessageHandler handler) =>
            new InProcAgent(new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAgentFramework(builder =>
                        builder.AddIssuerAgent(config =>
                            {
                                config.EndpointUri = new Uri("http://test");
                                config.WalletConfiguration = new WalletConfiguration {Id = Guid.NewGuid().ToString()};
                                config.WalletCredentials = new WalletCredentials {Key = "test"};
                                config.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                                config.PoolName = "TestPool";
                            })
                            .AddSovrinToken());
                    services.AddSingleton<IHttpClientFactory>(new InProcFactory(handler));
                }).Build());

        #endregion

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            await Host.StartAsync();
            Context = await Host.Services.GetService<IAgentProvider>().GetContextAsync();
        }

        /// <inheritdoc />
        public Task DisposeAsync() => Host.StopAsync(TimeSpan.FromSeconds(10));

        public class PairedAgents
        {
            public InProcAgent Agent1 { get; set; }

            public InProcAgent Agent2 { get; set; }

            public ConnectionRecord Connection1 { get; set; }

            public ConnectionRecord Connection2 { get; set; }
        }

        public class InProcFactory : IHttpClientFactory
        {
            public InProcFactory(HttpMessageHandler handler)
            {
                Handler = handler;
            }

            public HttpMessageHandler Handler { get; set; }

            /// <inheritdoc />
            public HttpClient CreateClient(string name)
            {
                return new HttpClient(Handler);
            }
        }
    }
}