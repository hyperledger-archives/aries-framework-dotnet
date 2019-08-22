using System;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Wallets;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultProvisioningService : IProvisioningService
    {
        /// <summary>The record service</summary>
        // ReSharper disable InconsistentNaming
        protected readonly IWalletRecordService RecordService;

        /// <summary>The wallet service</summary>
        protected readonly IWalletService WalletService; 
        // ReSharper restore InconsistentNaming

        /// <summary>Initializes a new instance of the <see cref="DefaultProvisioningService"/> class.</summary>
        /// <param name="walletRecord">The wallet record.</param>
        /// <param name="walletService">The wallet service.</param>
        public DefaultProvisioningService(
            IWalletRecordService walletRecord, 
            IWalletService walletService)
        {
            RecordService = walletRecord;
            WalletService = walletService;
        }

        /// <inheritdoc />
        public virtual async Task<ProvisioningRecord> GetProvisioningAsync(Wallet wallet)
        {
            var record = await RecordService.GetAsync<ProvisioningRecord>(wallet, ProvisioningRecord.UniqueRecordId);

            if (record == null)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Provisioning record not found");

            return record;
        }

        /// <inheritdoc />
        [Obsolete]
        public virtual async Task ProvisionAgentAsync(Wallet wallet, ProvisioningConfiguration provisioningConfiguration)
        {
            if (provisioningConfiguration == null)
                throw new ArgumentNullException(nameof(provisioningConfiguration));
            if (provisioningConfiguration.EndpointUri == null)
                throw new ArgumentNullException(nameof(provisioningConfiguration.EndpointUri));

            ProvisioningRecord record = null;
            try
            {
                record = await GetProvisioningAsync(wallet);
            }
            catch (AgentFrameworkException e) when(e.ErrorCode == ErrorCode.RecordNotFound){}

            if (record != null)
                throw new AgentFrameworkException(ErrorCode.WalletAlreadyProvisioned);

            var agent = await Did.CreateAndStoreMyDidAsync(wallet,
                provisioningConfiguration.AgentSeed != null
                    ? new {seed = provisioningConfiguration.AgentSeed}.ToJson()
                    : "{}");

            var masterSecretId = await AnonCreds.ProverCreateMasterSecretAsync(wallet, null);

            record = new ProvisioningRecord
            {
                MasterSecretId = masterSecretId,
                Endpoint =
                {
                    Uri = provisioningConfiguration.EndpointUri.ToString(),
                    Did = agent.Did,
                    Verkey = agent.VerKey
                },
                Owner =
                {
                    Name = provisioningConfiguration.OwnerName,
                    ImageUrl = provisioningConfiguration.OwnerImageUrl
                }
            };

            await provisioningConfiguration.ConfigureAsync(record, new DefaultAgentContext { Wallet = wallet });

            await RecordService.AddAsync(wallet, record);
        }

        /// <inheritdoc />
        public virtual async Task ProvisionAgentAsync(ProvisioningConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (configuration.WalletConfiguration == null ||
                configuration.WalletCredentials == null)
                throw new ArgumentNullException(nameof(configuration),
                    "Wallet configuration and credentials must be specified");

            // Create agent wallet
            await WalletService.CreateWalletAsync(configuration.WalletConfiguration, configuration.WalletCredentials);
            var wallet =
                await WalletService.GetWalletAsync(configuration.WalletConfiguration, configuration.WalletCredentials);

            // Configure agent endpoint
            AgentEndpoint endpoint = null;
            if (configuration.EndpointUri != null)
            {
                endpoint = new AgentEndpoint { Uri = configuration.EndpointUri?.ToString() };
                if (configuration.AgentSeed != null)
                {
                    var agent = await Did.CreateAndStoreMyDidAsync(wallet, new { seed = configuration.AgentSeed }.ToJson());
                    endpoint.Did = agent.Did;
                    endpoint.Verkey = agent.VerKey;
                }
                else if (configuration.AgentDid != null && configuration.AgentVerkey != null)
                {
                    endpoint.Did = configuration.AgentDid;
                    endpoint.Verkey = configuration.AgentVerkey;
                }
                else
                {
                    var agent = await Did.CreateAndStoreMyDidAsync(wallet, "{}");
                    endpoint.Did = agent.Did;
                    endpoint.Verkey = agent.VerKey;
                }
            }

            var masterSecretId = await AnonCreds.ProverCreateMasterSecretAsync(wallet, null);

            var record = new ProvisioningRecord
            {
                MasterSecretId = masterSecretId,
                Endpoint = endpoint,
                Owner =
                {
                    Name = configuration.OwnerName,
                    ImageUrl = configuration.OwnerImageUrl
                }
            };

            // Populate initial tags if any passed
            if (configuration.Tags != null && configuration.Tags.Any())
                foreach (var item in configuration.Tags)
                    record.Tags.Add(item.Key, item.Value);

            // Create issuer
            await configuration.ConfigureAsync(record, new DefaultAgentContext { Wallet = wallet });

            await RecordService.AddAsync(wallet, record);
        }

        /// <inheritdoc />
        public virtual async Task UpdateEndpointAsync(Wallet wallet, AgentEndpoint endpoint)
        {
            var record = await GetProvisioningAsync(wallet);
            record.Endpoint = endpoint;

            await RecordService.UpdateAsync(wallet, record);
        }
    }
}