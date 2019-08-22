using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Models;
using Microsoft.Extensions.Options;

namespace AgentFramework.Core.Handlers.Agents
{
    /// <inheritdoc />
    public class DefaultAgentProvider : IAgentProvider
    {
        private readonly WalletOptions _walletOptions;
        private readonly PoolOptions _poolOptions;
        private readonly IAgent _defaultAgent;
        private readonly IWalletService _walletService;
        private readonly IPoolService _poolService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAgentProvider"/> class.
        /// </summary>
        /// <param name="walletOptions">Wallet options.</param>
        /// <param name="poolOptions">Pool options.</param>
        /// <param name="defaultAgent">Default agent.</param>
        /// <param name="walletService">Wallet service.</param>
        /// <param name="poolService">Pool service.</param>
        public DefaultAgentProvider(
            IOptions<WalletOptions> walletOptions,
            IOptions<PoolOptions> poolOptions,
            IAgent defaultAgent,
            IWalletService walletService,
            IPoolService poolService)
        {
            _walletOptions = walletOptions.Value;
            _poolOptions = poolOptions.Value;
            _defaultAgent = defaultAgent;
            _walletService = walletService;
            _poolService = poolService;
        }

        /// <inheritdoc />
        public Task<IAgent> GetAgentAsync(params object[] args)
        {
            return Task.FromResult(_defaultAgent);
        }

        /// <inheritdoc />
        public async Task<IAgentContext> GetContextAsync(params object[] args)
        {
            var agent = await GetAgentAsync(args);
            return new DefaultAgentContext
            {
                Wallet = await _walletService.GetWalletAsync(_walletOptions.WalletConfiguration,
                    _walletOptions.WalletCredentials),
                Pool = new PoolAwaitable(() => _poolService.GetPoolAsync(
                    _poolOptions.PoolName, _poolOptions.ProtocolVersion)),
                SupportedMessages = agent.GetSupportedMessageTypes()
            };
        }
    }
}
