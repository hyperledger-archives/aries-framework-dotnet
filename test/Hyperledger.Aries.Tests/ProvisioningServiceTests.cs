using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Tests
{
    public class ProvisioningServiceTests : IAsyncLifetime
    {
        private WalletConfiguration _config = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        private WalletCredentials _creds = new WalletCredentials { Key = "1" };

        private DefaultWalletService _walletService;
        private DefaultProvisioningService _provisioningService;

        public async Task DisposeAsync()
        {
            await _walletService.DeleteWalletAsync(_config, _creds);
        }

        public Task InitializeAsync()
        {
            _walletService = new DefaultWalletService();
            _provisioningService = new DefaultProvisioningService(
                new DefaultWalletRecordService(),
                _walletService,
                Options.Create(new AgentOptions
                {
                    WalletConfiguration = _config,
                    WalletCredentials = _creds
                }));

            return Task.CompletedTask;
        }

        [Fact]
        public async Task ProvisionNewWalletWithEndpoint()
        {
            await _provisioningService.ProvisionAgentAsync(
                new AgentOptions
                {
                    EndpointUri = "http://mock",
                    WalletConfiguration = _config,
                    WalletCredentials = _creds
                });

            var wallet = await _walletService.GetWalletAsync(_config, _creds);
            Assert.NotNull(wallet);

            var provisioning = await _provisioningService.GetProvisioningAsync(wallet);

            Assert.NotNull(provisioning);
            Assert.NotNull(provisioning.Endpoint);
            Assert.NotNull(provisioning.Endpoint.Did);
            Assert.NotNull(provisioning.Endpoint.Verkey);
        }

        [Fact]
        public async Task ProvisionNewWalletWithoutEndpoint()
        {
            var walletService = new DefaultWalletService();
            var provisioningService = new DefaultProvisioningService(
                new DefaultWalletRecordService(),
                walletService,
                Options.Create(new AgentOptions
                {
                    WalletConfiguration = _config,
                    WalletCredentials = _creds
                }));

            await provisioningService.ProvisionAgentAsync();

            var wallet = await walletService.GetWalletAsync(_config, _creds);
            Assert.NotNull(wallet);

            var provisioning = await provisioningService.GetProvisioningAsync(wallet);

            Assert.NotNull(provisioning);
            Assert.Null(provisioning.Endpoint.Uri);
        }

        [Fact]
        public async Task ProvisionNewWalletCanUpdateEndpoint()
        {
            var walletService = new DefaultWalletService();
            var provisioningService = new DefaultProvisioningService(
                new DefaultWalletRecordService(),
                walletService,
                Options.Create(new AgentOptions
                {
                    WalletConfiguration = _config,
                    WalletCredentials = _creds
                }));

            await provisioningService.ProvisionAgentAsync();

            var wallet = await walletService.GetWalletAsync(_config, _creds);
            Assert.NotNull(wallet);

            var provisioning = await provisioningService.GetProvisioningAsync(wallet);

            Assert.NotNull(provisioning);
            Assert.Null(provisioning.Endpoint.Uri);

            await provisioningService.UpdateEndpointAsync(wallet, new AgentEndpoint
            {
                Uri = "http://mock"
            });

            provisioning = await provisioningService.GetProvisioningAsync(wallet);

            Assert.NotNull(provisioning);
            Assert.NotNull(provisioning.Endpoint);
            Assert.NotNull(provisioning.Endpoint.Uri);
        }
    }
}
