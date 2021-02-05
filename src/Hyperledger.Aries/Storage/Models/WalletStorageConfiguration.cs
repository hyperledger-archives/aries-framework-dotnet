using Newtonsoft.Json;

namespace Hyperledger.Aries.Storage
{
    public partial class WalletConfiguration
    {
        /// <summary>
        /// Wallet storage configuration.
        /// </summary>
        public class WalletStorageConfiguration
        {
            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            [JsonProperty("path")]
            public string Path { get; set; }

            /// <summary>
            /// Gets or sets the database host url.
            /// </summary>
            /// <value>The url.</value>
            [JsonProperty("url")]
            public string Url { get; set; }

            /// <summary>
            /// Gets or sets the database wallet scheme.
            /// </summary>
            /// <value>The wallet scheme.</value>
            [JsonProperty("wallet_scheme")]
            public string WalletScheme { get; set; }

            /// <inheritdoc />
            public override string ToString() =>
                $"{GetType().Name}: " +
                $"Path={Path}" +
                $"Url={Url}" +
                $"WalletScheme={WalletScheme}";
        }
    }
}
