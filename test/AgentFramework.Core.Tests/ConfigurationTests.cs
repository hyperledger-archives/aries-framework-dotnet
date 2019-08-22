using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.AspNetCore.Middleware;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Models.Wallets;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;
using System.Net;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Runtime;

namespace AgentFramework.Core.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void AddAgentframeworkInjectsRequiredServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLogging();
            services.AddAgentFramework();

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
            services.AddAgentFramework();
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
            services.AddAgentFramework();
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
                .Setup(x => x.ProvisionAgentAsync(It.IsAny<ProvisioningConfiguration>()))
                .Callback(() => { slim.Release(); provisioned = true; })
                .Returns(Task.CompletedTask);

            var hostBuilder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);
                    services.AddAgentFramework(b => b.AddBasicAgent(c => { }));
                    services.AddSingleton(provisioningMock.Object);
                })
                .Build();

            // Start the host
            await hostBuilder.StartAsync();

            // Wait for semaphore
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            await hostBuilder.StopAsync();

            // Assert
            provisioned.Should().BeTrue();
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
                    services.AddAgentFramework(b => b.AddIssuerAgent(c =>
                    {
                        c.WalletCredentials = walletCredentials;
                        c.WalletConfiguration = walletConfiguration;
                        c.EndpointUri = new Uri("http://example.com");
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
                .Returns(Task.FromResult<MessageResponse>(null));

            // Arrange
            var middleware = new AgentMiddleware(mockedContext.Object);

            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream();
            context.Request.Method = HttpMethod.Post.Method;
            context.Request.ContentType = DefaultMessageService.AgentWireMessageMimeType;
            context.Request.ContentLength = 1;

            await context.Request.Body.WriteAsync(new ReadOnlyMemory<byte>(new { dummy = "dummy" }.ToJson().GetUTF8Bytes()));
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            //Act
            await middleware.InvokeAsync(context, next => Task.CompletedTask);

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
            var middleware = new AgentMiddleware(mockedContext.Object);

            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream();
            context.Request.Method = HttpMethod.Put.Method;

            await context.Response.Body.WriteAsync(new ReadOnlyMemory<byte>(new { dummy = "dummy" }.ToJson().GetUTF8Bytes()));

            //Act
            Func<Task> act = async () => await middleware.InvokeAsync(context, next =>
            {
                nextInvoked = true;
                return Task.CompletedTask;
            });

            await act.Should().NotThrowAsync();
            nextInvoked.Should().BeTrue();
        }
    }
}
