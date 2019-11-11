
using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using Hyperledger.Indy.PoolApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgentFramework.Core.Configuration
{
    public class PoolConfigurationHostedService : IHostedService
    {
        private readonly PoolOptions _poolOptions;
        private readonly IPoolService _poolService;
        private readonly ILogger<PoolConfigurationHostedService> _logger;

        public PoolConfigurationHostedService(
            IOptions<PoolOptions> options,
            IPoolService poolService,
            ILogger<PoolConfigurationHostedService> logger)
        {
            _poolOptions = options.Value;
            _poolService = poolService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_poolOptions.GenesisFilename == null)
                {
                    throw new ArgumentNullException(nameof(_poolOptions.GenesisFilename), "You must specify the pool configuration genesis file.");
                }
                await _poolService.CreatePoolAsync(_poolOptions.PoolName, _poolOptions.GenesisFilename);
            }
            catch (PoolLedgerConfigExistsException)
            {
                // Pool already exists, swallow exception
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Couldn't create ledger configuration");
                throw;
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}