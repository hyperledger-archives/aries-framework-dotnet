using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Agents
{
    /// <inheritdoc />
    public class DefaultAgentProvider : IAgentProvider
    {
        private readonly AgentOptions _agentOptions;
        private readonly IAgent _defaultAgent;
        private readonly IWalletService _walletService;
        private readonly IPoolService _poolService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAgentProvider"/> class.
        /// </summary>
        /// <param name="agentOptions"></param>
        /// <param name="defaultAgent">Default agent.</param>
        /// <param name="walletService">Wallet service.</param>
        /// <param name="poolService">Pool service.</param>
        public DefaultAgentProvider(
            IOptions<AgentOptions> agentOptions,
            IAgent defaultAgent,
            IWalletService walletService,
            IPoolService poolService)
        {
            _agentOptions = agentOptions.Value;
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
                Wallet = await _walletService.GetWalletAsync(
                    configuration: _agentOptions.WalletConfiguration,
                    credentials: _agentOptions.WalletCredentials),
                Pool = new PoolAwaitable(() => _poolService.GetPoolAsync(
                    poolName: _agentOptions.PoolName,
                    protocolVersion: _agentOptions.ProtocolVersion)),
                SupportedMessages = agent.GetSupportedMessageTypes(),
                Agent = await GetAgentAsync(args),
                UseMessageTypesHttps = _agentOptions.UseMessageTypesHttps
            };
        }
    }
}
