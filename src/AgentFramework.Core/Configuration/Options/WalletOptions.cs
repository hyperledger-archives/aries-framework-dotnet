using AgentFramework.Core.Models.Wallets;

namespace AgentFramework.Core.Configuration.Options
{
    /// <summary>Wallet options</summary>
    public class WalletOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WalletOptions"/> class.
        /// </summary>
        public WalletOptions()
        {
            WalletConfiguration = new WalletConfiguration { Id = "DefaultWallet" };
            WalletCredentials = new WalletCredentials { Key = "DefaultKey" };
        }

        /// <summary>
        /// Gets or sets the wallet configuration.
        /// </summary>
        /// <value>The wallet configuration.</value>
        public WalletConfiguration WalletConfiguration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the wallet credentials.
        /// </summary>
        /// <value>The wallet credentials.</value>
        public WalletCredentials WalletCredentials
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"WalletConfiguration={WalletConfiguration}, " +
            $"WalletCredentials={WalletCredentials}";
    }
}
