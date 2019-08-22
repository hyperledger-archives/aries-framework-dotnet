using AgentFramework.Core.Models.Wallets;
using System;
using System.Threading.Tasks;
using AgentFramework.Core.Runtime;
using Xunit;

namespace AgentFramework.Core.Tests
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
                new DefaultWalletRecordService(), _walletService);

            return Task.CompletedTask;
        }

        [Fact]
        public async Task ProvisionNewWalletWithEndpoint()
        {
            await _provisioningService.ProvisionAgentAsync(new BasicProvisioningConfiguration
            {
                WalletConfiguration = _config,
                WalletCredentials = _creds,
                EndpointUri = new Uri("http://mock")
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
                new DefaultWalletRecordService(), walletService);

            await provisioningService.ProvisionAgentAsync(new BasicProvisioningConfiguration
            {
                WalletConfiguration = _config,
                WalletCredentials = _creds
            });

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
                new DefaultWalletRecordService(), walletService);

            await provisioningService.ProvisionAgentAsync(new BasicProvisioningConfiguration
            {
                WalletConfiguration = _config,
                WalletCredentials = _creds
            });

            var wallet = await walletService.GetWalletAsync(_config, _creds);
            Assert.NotNull(wallet);

            var provisioning = await provisioningService.GetProvisioningAsync(wallet);

            Assert.NotNull(provisioning);
            Assert.Null(provisioning.Endpoint.Uri);

            await provisioningService.UpdateEndpointAsync(wallet, new Models.AgentEndpoint
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
