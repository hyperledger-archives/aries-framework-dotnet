using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Represents a requested proof object
    /// </summary>
    public class RequestedProof
    {
        /// <summary>
        /// Gets or sets the revealed attributes.
        /// </summary>
        /// <value>
        /// The revealed attributes.
        /// </value>
        [JsonProperty("revealed_attrs")]
        [JsonPropertyName("revealed_attrs")]
        public Dictionary<string, ProofAttribute> RevealedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the revealed attributes.
        /// </summary>
        /// <value>
        /// The revealed attributes.
        /// </value>
        [JsonProperty("self_attested_attrs")]
        [JsonPropertyName("self_attested_attrs")]
        public Dictionary<string, string> SelfAttestedAttributes { get; set; }
    }
}
