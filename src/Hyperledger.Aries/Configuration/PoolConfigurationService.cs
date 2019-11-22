
using System;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Indy.PoolApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Configuration
{
    /// <summary>
    /// Pool configuration service registered as hosted service on startup
    /// to create the required pool configuration
    /// </summary>
    public class PoolConfigurationService : IHostedService
    {
        private readonly AgentOptions _agentOptions;
        private readonly IPoolService _poolService;
        private readonly ILogger<PoolConfigurationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolConfigurationService" /> class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="poolService"></param>
        /// <param name="logger"></param>
        public PoolConfigurationService(
            IOptions<AgentOptions> options,
            IPoolService poolService,
            ILogger<PoolConfigurationService> logger)
        {
            _agentOptions = options.Value;
            _poolService = poolService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_agentOptions.GenesisFilename == null)
                {
                    _logger.LogWarning("Pool configuration genesis file not supplied.");
                    return;
                }
                await _poolService.CreatePoolAsync(_agentOptions.PoolName, _agentOptions.GenesisFilename);
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