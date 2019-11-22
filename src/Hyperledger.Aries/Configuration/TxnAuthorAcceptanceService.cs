using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Agents;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Configuration
{
    /// <summary>
    /// Transaction author acceptance service registered as hosted service on startup
    /// to check if acceptance is required and accept if needed.
    /// The acceptance trail (digest and timestamp) is saved in the <see cref="ProvisioningRecord" />
    /// </summary>
    public class TxnAuthorAcceptanceService : IHostedService
    {
        private readonly IProvisioningService provisioningService;
        private readonly IWalletRecordService recordService;
        private readonly IPoolService _poolService;
        private readonly IAgentProvider _agentProvider;
        private readonly AgentOptions agentOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TxnAuthorAcceptanceService" /> class.
        /// </summary>
        /// <param name="applicationLifetime"></param>
        /// <param name="provisioningService"></param>
        /// <param name="recordService"></param>
        /// <param name="poolService"></param>
        /// <param name="agentProvider"></param>
        /// <param name="agentOptions"></param>
        public TxnAuthorAcceptanceService(
            IApplicationLifetime applicationLifetime,
            IProvisioningService provisioningService,
            IWalletRecordService recordService,
            IPoolService poolService,
            IAgentProvider agentProvider,
            IOptions<AgentOptions> agentOptions)
        {
            applicationLifetime.ApplicationStarted.Register(AcceptTxnAuthorAgreement);
            this.provisioningService = provisioningService;
            this.recordService = recordService;
            _poolService = poolService;
            _agentProvider = agentProvider;
            this.agentOptions = agentOptions.Value;
        }

        private async void AcceptTxnAuthorAgreement()
        {
            var context = await _agentProvider.GetContextAsync(nameof(TxnAuthorAcceptanceService));
            var taa = await _poolService.GetTaaAsync(agentOptions.PoolName);
            if (taa != null)
            {
                var digest = GetDigest(taa);
                var provisioning = await provisioningService.GetProvisioningAsync(context.Wallet);

                if (provisioning.TaaAcceptance == null || provisioning.TaaAcceptance.Digest != digest)
                {
                    await provisioningService.AcceptTxnAuthorAgreementAsync(context.Wallet, taa);
                }
            }
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private string GetDigest(IndyTaa taa)
        {
            using(var shaAlgorithm = SHA256.Create())
            return shaAlgorithm.ComputeHash(
                $"{taa.Version}{taa.Text}"
                .GetUTF8Bytes())
            .ToHexString();
        }
    }
}