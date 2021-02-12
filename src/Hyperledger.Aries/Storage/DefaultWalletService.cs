using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Extensions;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Storage
{
    /// <inheritdoc />
    public class DefaultWalletService : IWalletService
    {
        /// <summary>
        /// Dictionary of open wallets
        /// </summary>
        protected static readonly ConcurrentDictionary<string, Wallet> Wallets =
            new ConcurrentDictionary<string, Wallet>();

        /// <summary>
        /// Mutex semaphore for opening a new (not cached) wallet
        /// </summary>
        private static readonly SemaphoreSlim OpenWalletSemaphore = new SemaphoreSlim(1,1);

        /// <inheritdoc />
        public virtual async Task<Wallet> GetWalletAsync(WalletConfiguration configuration, WalletCredentials credentials)
        {
            var wallet = GetWalletFromCache(configuration);

            if (wallet == null)
            {
                wallet = await OpenWalletWithMutexAsync(configuration, credentials);
            }

            return wallet;
        }

        private async Task<Wallet> OpenWalletWithMutexAsync(WalletConfiguration configuration, WalletCredentials credentials)
        {
            Wallet wallet;

            await OpenWalletSemaphore.WaitAsync();
            try
            {
                wallet = GetWalletFromCache(configuration);

                if (wallet == null)
                {
                    wallet = await Wallet.OpenWalletAsync(configuration.ToJson(), credentials.ToJson());
                    Wallets.TryAdd(configuration.Id, wallet);
                }
            }
            finally
            {
                OpenWalletSemaphore.Release();
            }

            return wallet;
        }

        private Wallet GetWalletFromCache(WalletConfiguration configuration)
        {
            if (Wallets.TryGetValue(configuration.Id, out var wallet))
            {
                if (wallet.IsOpen)
                    return wallet;

                Wallets.TryRemove(configuration.Id, out wallet);
            }
            return null;
        }

        /// <inheritdoc />
        public virtual async Task CreateWalletAsync(WalletConfiguration configuration, WalletCredentials credentials)
        {
            await Wallet.CreateWalletAsync(configuration.ToJson(), credentials.ToJson());
        }

        /// <inheritdoc />
        public virtual async Task DeleteWalletAsync(WalletConfiguration configuration, WalletCredentials credentials)
        {
            if (Wallets.TryRemove(configuration.Id, out var wallet))
            {
                if (wallet.IsOpen)
                    await wallet.CloseAsync();

                wallet.Dispose();
            }
            await Wallet.DeleteWalletAsync(configuration.ToJson(), credentials.ToJson());
        }
    }
}
