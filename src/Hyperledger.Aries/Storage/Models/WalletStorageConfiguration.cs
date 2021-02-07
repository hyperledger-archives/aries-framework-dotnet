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

            /// <summary>
            /// Gets or sets the database name.
            /// </summary>
            /// <value>The database name.</value>
            [JsonProperty("database_name")]
            public string DatabaseName { get; set; }

            /// <summary>
            /// Gets or sets the tls.
            /// </summary>
            /// <value>tls (on|off).</value>
            [JsonProperty("tls")]
            public string Tls { get; set; } = "off";

            /// <summary>
            /// Gets or sets max connections.
            /// </summary>
            /// <value>The max connections.</value>
            [JsonProperty("max_connections")]
            public int? MaxConnections { get; set; }

            /// <summary>
            /// Gets or sets minimum idle count.
            /// </summary>
            /// <value>The minimum idle count.</value>
            [JsonProperty("min_idle_count")]
            public int? MinIdleCount { get; set; }

            /// <summary>
            /// Gets or sets connection timeout.
            /// </summary>
            /// <value>The conncection timeout.</value>
            [JsonProperty("connection_timeout")]
            public int? ConnectionTimeout { get; set; }

            /// <inheritdoc />
            public override string ToString() =>
                $"{GetType().Name}: " +
                $"Path={Path}" +
                $"Url={Url}" +
				$"Tls={Tls}" +
                $"WalletScheme={WalletScheme}" +
				$"DatabaseName={DatabaseName}" +
				$"MaxConnections={MaxConnections}" +
				$"MinIdleCount={MinIdleCount}" +
				$"ConnectionTimeout={ConnectionTimeout}";
        }
    }
}
