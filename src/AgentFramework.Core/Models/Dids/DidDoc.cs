using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Dids
{
    /// <summary>
    /// Strongly type DID doc model.
    /// </summary>
    public class DidDoc
    {
        /// <summary>
        /// The DID doc context.
        /// </summary>
        [JsonProperty("@context")]
        public string Context { get; set; } = "https://w3id.org/did/v1";

        /// <summary>
        /// List of public keys available on the DID doc.
        /// </summary>
        [JsonProperty("publicKey")]
        public IList<DidDocKey> Keys { get; set; }

        /// <summary>
        /// List of services available on the did doc.
        /// </summary>
        [JsonProperty("service")]
        [JsonConverter(typeof(DidDocServiceEndpointsConverter))]
        public IList<IDidDocServiceEndpoint> Services { get; set; } = new List<IDidDocServiceEndpoint>();
    }
}
