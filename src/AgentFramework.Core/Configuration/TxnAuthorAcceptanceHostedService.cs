using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Records;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AgentFramework.Core.Configuration
{
    public class TxnAuthorAcceptanceHostedService : IHostedService
    {
        private readonly IProvisioningService provisioningService;
        private readonly IWalletRecordService recordService;
        private readonly IPoolService _poolService;
        private readonly IAgentProvider _agentProvider;
        private readonly PoolOptions poolOptions;

        public TxnAuthorAcceptanceHostedService(
            IApplicationLifetime applicationLifetime,
            IProvisioningService provisioningService,
            IWalletRecordService recordService,
            IPoolService poolService,
            IAgentProvider agentProvider,
            IOptions<PoolOptions> poolOptions)
        {
            applicationLifetime.ApplicationStarted.Register(AcceptTxnAuthorAgreement);
            this.provisioningService = provisioningService;
            this.recordService = recordService;
            _poolService = poolService;
            _agentProvider = agentProvider;
            this.poolOptions = poolOptions.Value;
        }

        private async void AcceptTxnAuthorAgreement()
        {
            if (!poolOptions.AcceptTxnAuthorAgreement) return;

            var context = await _agentProvider.GetContextAsync(nameof(TxnAuthorAcceptanceHostedService));
            var taa = await _poolService.GetTaaAsync(poolOptions.PoolName);
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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