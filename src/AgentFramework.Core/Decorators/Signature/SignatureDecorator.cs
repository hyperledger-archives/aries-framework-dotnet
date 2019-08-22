using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Signature
{
    /// <summary>
    /// Signature decorator model.
    /// </summary>
    public class SignatureDecorator
    {
        /// <summary>
        /// Signature type.
        /// </summary>
        [JsonProperty("@type")]
        public string SignatureType { get; set; }

        /// <summary>
        /// Signature data.
        /// </summary>
        [JsonProperty("sig_data")]
        public string SignatureData { get; set; }

        /// <summary>
        /// Signer public key.
        /// </summary>
        [JsonProperty("signer")]
        public string Signer { get; set; }

        /// <summary>
        /// Signature.
        /// </summary>
        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
