using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Decorators.Signature
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
        [JsonPropertyName("@type")]
        public string SignatureType { get; set; }

        /// <summary>
        /// Signature data.
        /// </summary>
        [JsonProperty("sig_data")]
        [JsonPropertyName("sig_data")]
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
