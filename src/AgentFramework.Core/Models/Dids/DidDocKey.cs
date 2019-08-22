using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Dids
{
    /// <summary>
    /// Strongly type DID doc key model.
    /// </summary>
    public class DidDocKey
    {
        /// <summary>
        /// The id of the key.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of the key.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// The controller key.
        /// </summary>
        [JsonProperty("controller")]
        public string Controller { get; set; }

        /// <summary>
        /// The PEM representation of the key.
        /// </summary>
        [JsonProperty("publicKeyBase58")]
        public string PublicKeyBase58 { get; set; }

        //TODO add other public key representations
    }
}