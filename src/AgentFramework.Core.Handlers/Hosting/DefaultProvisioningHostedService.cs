using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AgentFramework.Core.Handlers.Hosting
{
    /// <inheritdoc />
    public class DefaultProvisioningHostedService : IHostedService
    {
        protected readonly IProvisioningService _provisioningService;
        protected readonly IWalletService _walletService;
        protected readonly IWalletRecordService _recordService;
        protected readonly WalletOptions _walletOptions;
        protected readonly AgentOptions _agentOptions;
        /// <inheritdoc />
        public DefaultProvisioningHostedService(
            IProvisioningService provisioningService,
            IWalletService walletService,
            IWalletRecordService recordService,
            IOptions<AgentOptions> agentOptions,
            IOptions<WalletOptions> walletOptions)
        {
            _provisioningService = provisioningService;
            _walletService = walletService;
            _recordService = recordService;
            _walletOptions = walletOptions.Value;
            _agentOptions = agentOptions.Value;
        }

        /// <inheritdoc />
        public async virtual Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await ProvisionAsync(cancellationToken);
            }
            catch(Exception)
            {
                // OK
            }
        }

        /// <summary>
        /// Create and provision agent, wallet, etc.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task ProvisionAsync(CancellationToken cancellationToken)
        {
            // Create agent wallet
            await _walletService.CreateWalletAsync(
                configuration: _walletOptions.WalletConfiguration,
                credentials: _walletOptions.WalletCredentials);
            var wallet = await _walletService.GetWalletAsync(
                configuration: _walletOptions.WalletConfiguration,
                credentials: _walletOptions.WalletCredentials);

            if (_agentOptions.AgentKeySeed == null)
            {
                _agentOptions.AgentKeySeed = CryptoUtils.GetUniqueKey(32);
            }

            // Configure agent endpoint
            AgentEndpoint endpoint = null;
            if (_agentOptions.EndpointUri != null)
            {
                endpoint = new AgentEndpoint { Uri = _agentOptions.EndpointUri?.ToString() };
                if (_agentOptions.AgentKeySeed != null)
                {
                    var agent = await Did.CreateAndStoreMyDidAsync(
                        wallet: wallet,
                        didJson: new
                        { 
                            seed = _agentOptions.AgentKeySeed, 
                            did = _agentOptions.AgentDid
                        }.ToJson());
                    endpoint.Did = agent.Did;
                    endpoint.Verkey = agent.VerKey;
                }
                else if (_agentOptions.AgentDid != null && _agentOptions.AgentKey != null)
                {
                    endpoint.Did = _agentOptions.AgentDid;
                    endpoint.Verkey = _agentOptions.AgentKey;
                }
            }

            var masterSecretId = await AnonCreds.ProverCreateMasterSecretAsync(wallet, null);

            var record = new ProvisioningRecord
            {
                MasterSecretId = masterSecretId,
                Endpoint = endpoint,
                Owner =
                {
                    Name = _agentOptions.AgentName,
                    ImageUrl = _agentOptions.AgentImageUri
                }
            };
            record.SetTag("AgentKeySeed", _agentOptions.AgentKeySeed);
            await _recordService.AddAsync(wallet, record);
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}