using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models.Wallets;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Hosting;

namespace AgentFramework.Core.Handlers.Hosting
{
    /// <inheritdoc />
    public class IssuerProvisioningHostedService : IHostedService
    {
        private readonly IProvisioningService _provisioningService;

        private IssuerProvisioningConfiguration Configuration { get; }
        
        /// <inheritdoc />
        public IssuerProvisioningHostedService(IProvisioningService provisioningService)
        {
            Configuration = new IssuerProvisioningConfiguration();
            _provisioningService = provisioningService;
        }
        /// <inheritdoc />
        public IssuerProvisioningHostedService(
            IProvisioningService provisioningService,
            IServiceProvider serviceProvider,
            Action<IServiceProvider, IssuerProvisioningConfiguration> configuration)
            : this(provisioningService)
        {
            if (configuration != null)
            {
                configuration(serviceProvider, Configuration);
            }
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _provisioningService.ProvisionAgentAsync(Configuration);
            }
            catch (WalletExistsException)
            {
                // Wallet already exists, swallow exception
            }
            catch (AgentFrameworkException ex) when (ex.ErrorCode == ErrorCode.WalletAlreadyProvisioned)
            {
                // Wallet already provisioned
            }
            catch (WalletStorageException)
            {
                // Aggregate exception thrown when using custom wallets

                // TODO: TM: add support to Indy SDK to expose exception types
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}