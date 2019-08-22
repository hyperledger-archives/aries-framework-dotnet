using System;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Wallets;
using AgentFramework.Core.Runtime;
using Hyperledger.Indy.WalletApi;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class WalletTests
    {
        [Fact]
        public async Task ConcurrentWalletAccess()
        {
            var walletService = new DefaultWalletService();

            var config = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
            var creds = new WalletCredentials { Key = "1" };

            await walletService.CreateWalletAsync(config, creds);

            Task<Wallet> openWalletTask1 = walletService.GetWalletAsync(config, creds);
            Task<Wallet> openWalletTask2 = walletService.GetWalletAsync(config, creds);
            Task<Wallet> openWalletTask3 = walletService.GetWalletAsync(config, creds);
            Task<Wallet> openWalletTask4 = walletService.GetWalletAsync(config, creds);

            await Task.WhenAll(openWalletTask1, openWalletTask2, openWalletTask3, openWalletTask4);

            Assert.True(openWalletTask1.Result.IsOpen);
            Assert.True(openWalletTask2.Result.IsOpen);
            Assert.True(openWalletTask3.Result.IsOpen);
            Assert.True(openWalletTask4.Result.IsOpen);
        }

        [Fact]
        public async Task CanCreateAndGetWallet()
        {
            var config = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
            var creds = new WalletCredentials { Key = "1" };

            var walletService = new DefaultWalletService();

            await walletService.CreateWalletAsync(config, creds);

            var wallet = await walletService.GetWalletAsync(config, creds);

            Assert.NotNull(wallet);
            Assert.True(wallet.IsOpen);
        }

        [Fact]
        public async Task CanCreateGetAndCloseWallet()
        {
            var config = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
            var creds = new WalletCredentials { Key = "1" };

            var walletService = new DefaultWalletService();

            await walletService.CreateWalletAsync(config, creds);

            var wallet = await walletService.GetWalletAsync(config, creds);

            Assert.NotNull(wallet);
            Assert.True(wallet.IsOpen);

            await wallet.CloseAsync();

            wallet = await walletService.GetWalletAsync(config, creds);

            Assert.NotNull(wallet);
            Assert.True(wallet.IsOpen);
        }

        [Fact]
        public async Task CanCreateGetAndDisposeWallet()
        {
            var config = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
            var creds = new WalletCredentials { Key = "1" };

            var walletService = new DefaultWalletService();

            await walletService.CreateWalletAsync(config, creds);

            var wallet = await walletService.GetWalletAsync(config, creds);

            Assert.NotNull(wallet);
            Assert.True(wallet.IsOpen);

            wallet.Dispose();

            wallet = await walletService.GetWalletAsync(config, creds);

            Assert.NotNull(wallet);
            Assert.True(wallet.IsOpen);
        }

        [Fact]
        public async Task CanCreateGetAndDeleteWallet()
        {
            var config = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
            var creds = new WalletCredentials { Key = "1" };

            var walletService = new DefaultWalletService();

            await walletService.CreateWalletAsync(config, creds);

            var wallet = await walletService.GetWalletAsync(config, creds);

            Assert.NotNull(wallet);
            Assert.True(wallet.IsOpen);
            
            await walletService.DeleteWalletAsync(config, creds);

            await Assert.ThrowsAsync<WalletNotFoundException>(() => walletService.GetWalletAsync(config, creds));
        }
    }
}
