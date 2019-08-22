using System;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Models.Wallets;
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
        private readonly ILogger<SovrinTokenConfigurationService> logger;

        public ProvisioningConfiguration AgentConfiguration { get; }

        public SovrinTokenConfigurationService(
            IApplicationLifetime applicationLifetime,
            IOptions<ProvisioningConfiguration> configuration,
            IPaymentService paymentService,
            IProvisioningService provisioningService,
            IAgentProvider agentProvider,
            IWalletRecordService recordService,
            ILogger<SovrinTokenConfigurationService> logger)
        {
            applicationLifetime.ApplicationStarted.Register(CreateDefaultPaymentAddress);
            AgentConfiguration = configuration.Value;
            this.paymentService = paymentService;
            this.provisioningService = provisioningService;
            this.agentProvider = agentProvider;
            this.recordService = recordService;
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
                    var address = await paymentService.CreatePaymentAddressAsync(context,
                        new AddressOptions
                        {
                            Seed = AgentConfiguration.AddressSeed,
                            Method = TokenConfiguration.MethodName
                        });
                    provisioning.DefaultPaymentAddressId = address.Id;
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