using System;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Payments.SovrinToken
{
    /// <summary>
    /// Sovrin Token Configuration Service
    /// </summary>
    internal class SovrinTokenConfigurationService : IHostedService
    {
        private readonly IPaymentService paymentService;
        private readonly IProvisioningService provisioningService;
        private readonly IAgentProvider agentProvider;
        private readonly IWalletRecordService recordService;
        private readonly AddressOptions addressOptions;
        private readonly ILogger<SovrinTokenConfigurationService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SovrinTokenConfigurationService" /> class.
        /// </summary>
        /// <param name="hostApplicationLifetime"></param>
        /// <param name="paymentService"></param>
        /// <param name="provisioningService"></param>
        /// <param name="agentProvider"></param>
        /// <param name="recordService"></param>
        /// <param name="addressOptions"></param>
        /// <param name="logger"></param>
        public SovrinTokenConfigurationService(
            IHostApplicationLifetime hostApplicationLifetime,
            IPaymentService paymentService,
            IProvisioningService provisioningService,
            IAgentProvider agentProvider,
            IWalletRecordService recordService,
            IOptions<AddressOptions> addressOptions,
            ILogger<SovrinTokenConfigurationService> logger)
        {
            hostApplicationLifetime.ApplicationStarted.Register(CreateDefaultPaymentAddress);
            this.paymentService = paymentService;
            this.provisioningService = provisioningService;
            this.agentProvider = agentProvider;
            this.recordService = recordService;
            this.addressOptions = addressOptions.Value;
            this.logger = logger;

        }

        private async void CreateDefaultPaymentAddress()
        {
            try
            {
                var context = await agentProvider.GetContextAsync();

                var provisioning = await provisioningService.GetProvisioningAsync(context.Wallet);
                if (provisioning.DefaultPaymentAddressId == null)
                {
                    if (addressOptions.Seed == null)
                    {
                        addressOptions.Seed = CryptoUtils.GetUniqueKey(32);
                    }
                    addressOptions.Method = "sov";
                    var address = await paymentService.CreatePaymentAddressAsync(context, addressOptions);

                    provisioning.DefaultPaymentAddressId = address.Id;
                    provisioning.SetTag("AddressSeed", addressOptions.Seed);
                    await recordService.UpdateAsync(context.Wallet, provisioning);
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Couldn't initialize default payment address");
            }
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken) => TokenConfiguration.InitializeAsync();

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
