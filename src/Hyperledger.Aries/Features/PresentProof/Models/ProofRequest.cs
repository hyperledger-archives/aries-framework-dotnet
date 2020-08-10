using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof request.
    /// </summary>
    public class ProofRequest
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        /// <value>The nonce.</value>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the requested attributes.
        /// </summary>
        /// <value>The requested attributes.</value>
        [JsonProperty("requested_attributes")]
        [JsonPropertyName("requested_attributes")]
        public Dictionary<string, ProofAttributeInfo> RequestedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the requested predicates.
        /// </summary>
        /// <value>The requested predicates.</value>
        [JsonProperty("requested_predicates", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("requested_predicates")]
        public Dictionary<string, ProofPredicateInfo> RequestedPredicates { get; set; } =
            new Dictionary<string, ProofPredicateInfo>();

        /// <summary>
        /// Gets or sets the non revoked.
        /// </summary>
        /// <value>The non revoked.</value>
        [JsonProperty("non_revoked", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("non_revoked")]
        public RevocationInterval NonRevoked { get; set; }
    }
}