using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Represents a partial proof stored in the wallet.
    /// </summary>
    public class PartialProof
    {
        /// <summary>
        /// Gets or sets the proof identifiers.
        /// </summary>
        /// <value>
        /// The proof identifiers.
        /// </value>
        [JsonProperty("identifiers")]
        public List<ProofIdentifier> Identifiers { get; set; }

        /// <summary>
        /// Gets or sets the requested proof.
        /// </summary>
        /// <value>
        /// The requested proof.
        /// </value>
        [JsonProperty("requested_proof")]
        [JsonPropertyName("requested_proof")]
        public RequestedProof RequestedProof { get; set; }
    }
}
