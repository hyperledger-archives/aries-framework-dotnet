using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Wallets
{
    /// <summary>
    /// Wallet credentials.
    /// </summary>
    public partial class WalletCredentials
    {
        /// <summary>
        /// Gets or sets the secret key used to derive wallet encryption key.
        /// </summary>
        /// <value>The key.</value>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// [Optional] Gets or sets the new key.
        /// </summary>
        /// <value>The new key.</value>
        [JsonProperty("rekey", NullValueHandling = NullValueHandling.Ignore)]
        public string NewKey { get; set; }

        /// <summary>
        /// Optional&lt;string> algorithm to use for master key derivation:
        ///                          ARAGON2I_MOD (used by default)
        ///                          ARAGON2I_INT - less secured but faster
        /// </summary>
        /// <value>The key derivation method.</value>
        [JsonProperty("key_derivation_method", NullValueHandling = NullValueHandling.Ignore)]
        public string KeyDerivationMethod { get; set; }

        /// <summary>
        /// [Optional] Gets or sets the storage credentials.
        /// </summary>
        /// <value>The storage credentials.</value>
        [JsonProperty("storage_credentials", NullValueHandling = NullValueHandling.Ignore)]
        public object StorageCredentials { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Key={(Key?.Length > 0 ? "[hidden]" : null)}, " +
            $"NewKey={(NewKey?.Length > 0 ? "[hidden]" : null)}, " +
            $"KeyDerivationMethod={KeyDerivationMethod}, " +
            $"StorageCredentials={(StorageCredentials != null ? "[hidden]" : null)}";
    }
}
