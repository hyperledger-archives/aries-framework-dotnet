using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Routing;
using Hyperledger.Aries.Routing.Mediator.Storage;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Hyperledger.TestHarness.Mock
{
    public class InProcMediatorAgent : InProcAgent
    {
        public InProcMediatorAgent(IHost host) : base(host)
        {
        }

        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddHandler<MediatorForwardHandler>();
            AddHandler<RoutingInboxHandler>();
            AddHandler<DefaultStoreBackupHandler>();
            AddHandler<RetrieveBackupHandler>();
        }

        internal Task<AgentPublicConfiguration> HandleDiscoveryAsync() =>
            Host.Services.GetRequiredService<MediatorDiscoveryMiddleware>().GetConfigurationAsync();
    }

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

        internal Task<MessageContext> HandleAsync(byte[] data) => 
            ProcessAsync(Context, new PackedMessageContext(data));

        /// <inheritdoc />
        protected override void ConfigureHandlers()
        {
            AddConnectionHandler();
            AddCredentialHandler();
            AddDidExchangeHandler();
            AddDiscoveryHandler();
            AddDiscoveryHandler();
            AddForwardHandler();
            AddProofHandler();
            AddRevocationNotificationHandler();
            AddBasicMessageHandler();
            AddHandler<RetrieveBackupHandler>();
            AddHandler<DefaultStoreBackupHandler>();
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

        public static async Task<PairedAgents> CreatePairedWithRoutingAsync(Dictionary<string, string> metaData = null)
        {
            var handler1 = new InProcMessageHandler();
            var handler2 = new InProcMessageHandler();

            var agent1 = CreateMediator(handler1);
            var agent2 = CreateEdge(handler2, metaData);

            handler1.TargetAgent = agent2;
            handler2.TargetAgent = agent1;

            await agent1.InitializeAsync();
            await agent2.InitializeAsync();

            var result = new PairedAgents
            {
                Agent1 = agent1,
                Agent2 = agent2
            };
            return result;
        }
        
        public static async Task<PairedAgents> CreateTwoWalletsPairedWithRoutingAsync()
        {
            var handler1 = new InProcMessageHandler();
            var handler2 = new InProcMessageHandler();
            var handler3 = new InProcMessageHandler();

            var agent1 = CreateMediator(handler1);
            var agent2 = CreateEdge(handler2);
            var agent3 = CreateEdge(handler3);

            handler1.TargetAgent = agent2;
            handler2.TargetAgent = agent1;
            handler3.TargetAgent = agent1;

            await agent1.InitializeAsync();
            await agent2.InitializeAsync();
            await agent3.InitializeAsync();

            var result = new PairedAgents
            {
                Agent1 = agent1,
                Agent2 = agent2,
                Agent3 = agent3
            };
            return result;
        }

        private static async Task<(ConnectionRecord agent1Connection, ConnectionRecord agent2Connection)> ConnectAsync(InProcAgent agent1, InProcAgent agent2)
        {
            var (invitation, agent1Connection) = await agent1.Provider.GetService<IConnectionService>().CreateInvitationAsync(agent1.Context, new InviteConfiguration { AutoAcceptConnection = true });

            var (request, agent2Connection) = await agent2.Provider.GetService<IConnectionService>().CreateRequestAsync(agent2.Context, invitation);
            await agent2.Provider.GetService<IMessageService>().SendAsync(
                agentContext: agent2.Context,
                message: request,
                recipientKey: invitation.RecipientKeys.First(),
                endpointUri: agent2Connection.Endpoint.Uri,
                routingKeys: agent2Connection.Endpoint.Verkey,
                senderKey: agent2Connection.MyVk);

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
                    services.AddAriesFramework(builder => builder
                        .RegisterAgent(options =>
                        {
                            options.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                            options.PoolName = "TestPool";
                            options.WalletConfiguration.Id = Guid.NewGuid().ToString();
                            options.WalletCredentials.Key = "test";
                            options.EndpointUri = "http://test";
                            options.RevocationRegistryDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                        }));
                    services.AddSingleton<IHttpClientFactory>(new InProcFactory(handler));
                    services.AddSingleton<IStorageService, DefaultStorageService>();
                    services.AddMessageHandler<DefaultStoreBackupHandler>();
                    services.AddMessageHandler<RetrieveBackupHandler>();
                }).Build());

        private static InProcAgent CreateEdge(HttpMessageHandler handler, Dictionary<string, string> metaData = null) =>
            new InProcAgent(new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAriesFramework(builder => builder
                        .RegisterEdgeAgent(options =>
                        {
                            options.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                            options.PoolName = "TestPool";
                            options.WalletConfiguration.Id = Guid.NewGuid().ToString();
                            options.WalletCredentials.Key = "test";
                            options.EndpointUri = "http://test";
                            options.MetaData = metaData;
                        }));
                    services.AddSingleton<IHttpClientFactory>(new InProcFactory(handler));
                }).Build());

        private static InProcAgent CreateMediator(HttpMessageHandler handler) =>
            new InProcMediatorAgent(new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAriesFramework(builder => builder
                        .RegisterMediatorAgent(options =>
                        {
                            options.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                            options.PoolName = "TestPool";
                            options.WalletConfiguration.Id = Guid.NewGuid().ToString();
                            options.WalletCredentials.Key = "test";
                            options.EndpointUri = "http://test";
                        }));
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
        public Task DisposeAsync()
        {
            Host.StopAsync(TimeSpan.FromSeconds(10));
            Host.Dispose();
            return Task.CompletedTask;
        }

        public class PairedAgents
        {
            public InProcAgent Agent1 { get; set; }

            public InProcAgent Agent2 { get; set; }
            public InProcAgent Agent3 { get; set; }

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
