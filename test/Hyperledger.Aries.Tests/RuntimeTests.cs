﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class RuntimeTests
    {
        [Fact]
        public void ResolveDependencyServices()
        {
            var services = new ServiceCollection();
            services.AddAriesFramework();
            
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
