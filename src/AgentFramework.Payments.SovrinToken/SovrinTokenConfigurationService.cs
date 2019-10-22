using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.Core.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgentFramework.Payments.SovrinToken
{
    internal class SovrinTokenConfigurationService : IHostedService
    {
        private readonly IPaymentService paymentService;
        private readonly IProvisioningService provisioningService;
        private readonly IAgentProvider agentProvider;
        private readonly IWalletRecordService recordService;
        private readonly AddressOptions addressOptions;
        private readonly ILogger<SovrinTokenConfigurationService> logger;

        public SovrinTokenConfigurationService(
            IApplicationLifetime applicationLifetime,
            IPaymentService paymentService,
            IProvisioningService provisioningService,
            IAgentProvider agentProvider,
            IWalletRecordService recordService,
            IOptions<AddressOptions> addressOptions,
            ILogger<SovrinTokenConfigurationService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(CreateDefaultPaymentAddress);
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return TokenConfiguration.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}