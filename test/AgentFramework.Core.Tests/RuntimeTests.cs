using AgentFramework.Core.Contracts;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class RuntimeTests
    {
        [Fact]
        public void ResolveDependancyServices()
        {
            var services = new ServiceCollection();
            services.AddAgentFramework();
            
            // Initialize Autofac
            var builder = new ContainerBuilder();
            builder.Populate(services);
            
            // Build the final container
            var container = builder.Build();

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
    }
}
