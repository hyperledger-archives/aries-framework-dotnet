using System.Threading.Tasks;
using AgentFramework.Core.Models.Wallets;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Wallet service.
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Gets the wallet async.
        /// </summary>
        /// <returns>The wallet async.</returns>
        /// <param name="configuration">Configuration.</param>
        /// <param name="credentials">Credentials.</param>
        Task<Wallet> GetWalletAsync(WalletConfiguration configuration, WalletCredentials credentials);

        /// <summary>
        /// Creates the wallet async.
        /// </summary>
        /// <returns>The wallet async.</returns>
        /// <param name="configuration">Configuration.</param>
        /// <param name="credentials">Credentials.</param>
        Task CreateWalletAsync(WalletConfiguration configuration, WalletCredentials credentials);

        /// <summary>
        /// Deletes the wallet async.
        /// </summary>
        /// <returns>Async Task</returns>
        /// <param name="configuration">Configuration.</param>
        /// <param name="credentials">Credentials.</param>
        Task DeleteWalletAsync(WalletConfiguration configuration, WalletCredentials credentials);
    }
}
