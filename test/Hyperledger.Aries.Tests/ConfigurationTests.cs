using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.AspNetCore;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.PoolApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void AddAgentframeworkInjectsRequiredServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();
            services.AddAriesFramework();

            // Initialize Autofac
            var builder = new ContainerBuilder();

            builder.Populate(services);

            // Build the final container
            var container = builder.Build();

            Assert.NotNull(container.Resolve<IEventAggregator>());
            Assert.NotNull(container.Resolve<IConnectionService>());
            Assert.NotNull(container.Resolve<ICredentialService>());
            Assert.NotNull(container.Resolve<IProofService>());
            Assert.NotNull(container.Resolve<ILedgerService>());
            Assert.NotNull(container.Resolve<ISchemaService>());
            Assert.NotNull(container.Resolve<IWalletRecordService>());
            Assert.NotNull(container.Resolve<IProvisioningService>());
            Assert.NotNull(container.Resolve<IMessageService>());
            Assert.NotNull(container.Resolve<ITailsService>());
            Assert.NotNull(container.Resolve<IWalletService>());
        }

        [Fact]
        public void AddAgentframeworkWithExtendedServiceResolves()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();
            services.AddAriesFramework();
            services.AddExtendedConnectionService<MockExtendedConnectionService>();

            // Initialize Autofac
            var builder = new ContainerBuilder();

            builder.Populate(services);

            // Build the final container
            var container = builder.Build();

            var result = container.Resolve<IConnectionService>();

            Assert.True(result.GetType() == typeof(MockExtendedConnectionService));
        }

        [Fact]
        public void AddAgentframeworkWithCustomHandler()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();
            services.AddAriesFramework();
            services.AddMessageHandler<MockMessageHandler>();

            // Initialize Autofac
            var builder = new ContainerBuilder();

            builder.Populate(services);

            // Build the final container
            var container = builder.Build();

            var result = container.Resolve<IMessageHandler>();

            Assert.True(result.GetType() == typeof(MockMessageHandler));
        }

        [Fact(DisplayName = "Verify the hosting service executed and provisioning completed")]
        public async Task RunHostingServiceEnsureProvisioningInvoked()
        {
            var slim = new SemaphoreSlim(0, 1);
            var provisioned = false;

            var provisioningMock = new Mock<IProvisioningService>();
            provisioningMock
                .Setup(x => x.ProvisionAgentAsync())
                .Callback(() =>
                {
                    provisioned = true;
                    slim.Release();
                })
                .Returns(Task.CompletedTask);

            var poolName = $"{Guid.NewGuid()}";
            var hostBuilder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAriesFramework(b => b.RegisterAgent(c =>
                    {
                        c.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                        c.ProtocolVersion = 2;
                        c.PoolName = poolName;
                    }));
                    services.AddSingleton(provisioningMock.Object);
                })
                .Build();

            // Start the host
            await hostBuilder.StartAsync();
            await slim.WaitAsync(TimeSpan.FromSeconds(30));

            // Assert
            var pool = await hostBuilder.Services.GetService<IPoolService>().GetPoolAsync(poolName);

            pool.Should().NotBeNull();
            provisioned.Should().BeTrue();

            // Cleanup
            await pool.CloseAsync();
            await Pool.DeletePoolLedgerConfigAsync(poolName);
            await hostBuilder.StopAsync();
        }

        [Fact(DisplayName = "Provisioning completed with issuer configuration")]
        public async Task RunHostingServiceWithIssuerProvisioning()
        {
            var walletConfiguration = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
            var walletCredentials = new WalletCredentials { Key = "key" };

            var hostBuilder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAriesFramework(b => b
                        .RegisterAgent(options =>
                        {
                            options.WalletCredentials = walletCredentials;
                            options.WalletConfiguration = walletConfiguration;
                            options.EndpointUri = "http://example.com";
                            options.GenesisFilename = Path.GetFullPath("pool_genesis.txn");
                        }));
                })
                .Build();

            // Start the host
            await hostBuilder.StartAsync();
            await hostBuilder.StopAsync();

            var walletService = hostBuilder.Services.GetService<IWalletService>();
            var wallet = await walletService.GetWalletAsync(walletConfiguration, walletCredentials);

            Assert.NotNull(wallet);

            var provisioningService = hostBuilder.Services.GetService<IProvisioningService>();
            var record = await provisioningService.GetProvisioningAsync(wallet);

            record.Should().NotBeNull();
            record.IssuerVerkey.Should().NotBeNull();
            record.Endpoint.Should().NotBeNull();
            record.Endpoint.Verkey.Should().NotBeNull();

            await wallet.CloseAsync();
            await walletService.DeleteWalletAsync(walletConfiguration, walletCredentials);
        }

        [Fact(DisplayName = "Agent middleware executed successfully with default agent")]
        public async Task AgentMiddlewareExecutedSuccessfully()
        {
            var mockedContext = new Mock<IAgentProvider>();
            var agentMock = new Mock<IAgent>();

            mockedContext.Setup(x => x.GetAgentAsync())
                .ReturnsAsync(agentMock.Object);
            agentMock.Setup(x => x.ProcessAsync(It.IsAny<IAgentContext>(), It.IsAny<MessageContext>()))
                .Returns(Task.FromResult<MessageContext>(null));

            // Arrange
            var middleware = new AgentMiddleware(next => Task.CompletedTask);

            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream();
            context.Request.Method = HttpMethod.Post.Method;
            context.Request.ContentType = DefaultMessageService.AgentWireMessageMimeType;
            context.Request.ContentLength = 1;

            await context.Request.Body.WriteAsync(new ReadOnlyMemory<byte>(new { dummy = "dummy" }.ToJson().GetUTF8Bytes()));
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            //Act
            await middleware.Invoke(context, mockedContext.Object);

            //Assert
            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.OK);
        }

        [Fact(DisplayName = "Agent middleware calls next delegate if invalid http method")]
        public async Task AgentMiddlewareCallsNextDelegateOnInvalidHttpMethod()
        {
            var mockedContext = new Mock<IAgentProvider>();
            var nextInvoked = false;

            // Arrange
            var middleware = new AgentMiddleware(next =>
            {
                nextInvoked = true;
                return Task.CompletedTask;
            });

            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream();
            context.Request.Method = HttpMethod.Put.Method;

            await context.Response.Body.WriteAsync(new ReadOnlyMemory<byte>(new { dummy = "dummy" }.ToJson().GetUTF8Bytes()));

            //Act
            Func<Task> act = async () => await middleware.Invoke(context, mockedContext.Object);

            await act.Should().NotThrowAsync();
            nextInvoked.Should().BeTrue();
        }
    }
}
