using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Wallets
{
    /// <summary>
    /// Wallet configuration.
    /// </summary>
    public partial class WalletConfiguration
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the storage.
        /// </summary>
        /// <value>The type of the storage.</value>
        [JsonProperty("storage_type")]
        public string StorageType { get; set; } = "default";

        /// <summary>
        /// Gets or sets the storage configuration.
        /// </summary>
        /// <value>The storage configuration.</value>
        [JsonProperty("storage_config", NullValueHandling = NullValueHandling.Ignore)]
        public WalletStorageConfiguration StorageConfiguration { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"StorageType={StorageType}, " +
            $"StorageConfiguration={StorageConfiguration}";
    }
}
