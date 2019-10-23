using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Options;

namespace AgentFramework.Core.Handlers.Hosting
{
    /// <inheritdoc />
    public class IssuerProvisioningHostedService : DefaultProvisioningHostedService
    {
        /// <inheritdoc />
        public IssuerProvisioningHostedService(
            IProvisioningService provisioningService, 
            IWalletService walletService, 
            IWalletRecordService recordService, 
            IOptions<AgentOptions> agentOptions, 
            IOptions<WalletOptions> walletOptions) 
            : base(
                provisioningService, 
                walletService, 
                recordService, 
                agentOptions, 
                walletOptions)
        {
        }

        /// <inheritdoc />
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await ProvisionAsync(cancellationToken);

                var wallet = await _walletService.GetWalletAsync(
                        configuration: _walletOptions.WalletConfiguration,
                        credentials: _walletOptions.WalletCredentials);

                if (_agentOptions.AgentKeySeed == null)
                {
                    _agentOptions.AgentKeySeed = CryptoUtils.GetUniqueKey(32);
                }

                var issuer = await Did.CreateAndStoreMyDidAsync(
                    wallet: wallet,
                    didJson: new
                    {
                        did = _agentOptions.IssuerDid,
                        seed = _agentOptions.IssuerKeySeed
                    }.ToJson());

                var record = await _provisioningService.GetProvisioningAsync(wallet);
                record.IssuerSeed = _agentOptions.IssuerKeySeed;
                record.IssuerDid = issuer.Did;
                record.IssuerVerkey = issuer.VerKey;
                //record.TailsBaseUri = TailsBaseUri?.ToString();

                await _recordService.UpdateAsync(wallet, record);
            }
            catch
            {
                // OK
            }
        }
    }
}