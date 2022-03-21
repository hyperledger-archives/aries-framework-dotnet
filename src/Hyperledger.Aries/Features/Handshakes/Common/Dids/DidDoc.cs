using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.Common.Dids
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
        [JsonPropertyName("@context")]
        public string Context { get; set; } = "https://w3id.org/did/v1";

        /// <summary>
        /// The ID of the DID doc
        /// </summary>
        /// <value></value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// List of public keys available on the DID doc.
        /// </summary>
        [JsonProperty("publicKey")]
        [JsonPropertyName("publicKey")]
        public IList<DidDocKey> Keys { get; set; }

        /// <summary>
        /// List of services available on the did doc.
        /// </summary>
        [JsonProperty("service")]
        [Newtonsoft.Json.JsonConverter(typeof(DidDocServiceEndpointsConverter))]
        // TODO STJ convertor needed
        public IList<IDidDocServiceEndpoint> Services { get; set; } = new List<IDidDocServiceEndpoint>();
    }
}
