using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Hosting;

namespace Hyperledger.Aries.Configuration
{
    /// <inheritdoc />
    internal class DefaultProvisioningHostedService : IHostedService
    {
        private readonly IProvisioningService _provisioningService;
        
        /// <inheritdoc />
        public DefaultProvisioningHostedService(IProvisioningService provisioningService)
        {
            _provisioningService = provisioningService;
        }

        /// <inheritdoc />
        public async virtual Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _provisioningService.ProvisionAgentAsync();
            }
            catch(WalletExistsException)
            {
                // OK
            }
            catch(WalletStorageException)
            {
                // OK
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}