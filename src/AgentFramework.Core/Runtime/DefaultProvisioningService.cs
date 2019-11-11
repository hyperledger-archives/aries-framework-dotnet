using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AgentFramework.Core.Configuration.Options;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Options;

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
        protected readonly AgentOptions AgentOptions;
        protected readonly WalletOptions WalletOptions;

        // ReSharper restore InconsistentNaming

        /// <summary>Initializes a new instance of the <see cref="DefaultProvisioningService"/> class.</summary>
        /// <param name="walletRecord">The wallet record.</param>
        /// <param name="walletService">The wallet service.</param>
        /// <param name="agentOptions"></param>
        /// <param name="walletOptions"></param>
        public DefaultProvisioningService(
            IWalletRecordService walletRecord, 
            IWalletService walletService,
            IOptions<AgentOptions> agentOptions,
            IOptions<WalletOptions> walletOptions)
        {
            RecordService = walletRecord;
            WalletService = walletService;
            AgentOptions = agentOptions.Value;
            WalletOptions = walletOptions.Value;
        }

        /// <inheritdoc />
        public async Task AcceptTxnAuthorAgreementAsync(Wallet wallet, IndyTaa txnAuthorAgreement)
        {
            var provisioning = await GetProvisioningAsync(wallet);

            provisioning.TaaAcceptance = new IndyTaaAcceptance
            {
                Digest = GetDigest(txnAuthorAgreement),
                Text = txnAuthorAgreement.Text,
                Version = txnAuthorAgreement.Version,
                AcceptanceDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            await RecordService.UpdateAsync(wallet, provisioning);
        }

        private string GetDigest(IndyTaa taa)
        {
            using(var shaAlgorithm = SHA256.Create())
            return shaAlgorithm.ComputeHash(
                $"{taa.Version}{taa.Text}"
                .GetUTF8Bytes())
            .ToHexString();
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

        /// <inheritdoc />
        public Task ProvisionAgentAsync() => ProvisionAgentAsync(AgentOptions, WalletOptions);

        /// <inheritdoc />
        public async Task ProvisionAgentAsync(AgentOptions agentOptions, WalletOptions walletOptions)
        {
            if (agentOptions is null)
            {
                throw new ArgumentNullException(nameof(agentOptions));
            }

            if (walletOptions is null)
            {
                throw new ArgumentNullException(nameof(walletOptions));
            }
            // Create agent wallet
            await WalletService.CreateWalletAsync(
                configuration: walletOptions.WalletConfiguration,
                credentials: walletOptions.WalletCredentials);
            var wallet = await WalletService.GetWalletAsync(
                configuration: walletOptions.WalletConfiguration,
                credentials: walletOptions.WalletCredentials);

            // Configure agent endpoint
            AgentEndpoint endpoint = null;
            if (agentOptions.EndpointUri != null)
            {
                endpoint = new AgentEndpoint { Uri = agentOptions.EndpointUri.ToString() };
                if (agentOptions.AgentKeySeed != null)
                {
                    var agent = await Did.CreateAndStoreMyDidAsync(wallet, new { seed = agentOptions.AgentKeySeed }.ToJson());
                    endpoint.Did = agent.Did;
                    endpoint.Verkey = agent.VerKey;
                }
                else if (agentOptions.AgentDid != null && agentOptions.AgentKey != null)
                {
                    endpoint.Did = agentOptions.AgentDid;
                    endpoint.Verkey = agentOptions.AgentKey;
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
                    Name = agentOptions.AgentName,
                    ImageUrl = agentOptions.AgentImageUri
                }
            };

            // Issuer Configuration
            if (agentOptions.IssuerKeySeed == null)
            {
                agentOptions.IssuerKeySeed = CryptoUtils.GetUniqueKey(32);
            }

            var issuer = await Did.CreateAndStoreMyDidAsync(
                wallet: wallet,
                didJson: new
                {
                    did = agentOptions.IssuerDid,
                    seed = agentOptions.IssuerKeySeed
                }.ToJson());

            record.IssuerSeed = agentOptions.IssuerKeySeed;
            record.IssuerDid = issuer.Did;
            record.IssuerVerkey = issuer.VerKey;

            record.SetTag("AgentKeySeed", agentOptions.AgentKeySeed);
            record.SetTag("IssuerKeySeed", agentOptions.IssuerKeySeed);

            // Add record to wallet
            await RecordService.AddAsync(wallet, record);
        }
    }
}