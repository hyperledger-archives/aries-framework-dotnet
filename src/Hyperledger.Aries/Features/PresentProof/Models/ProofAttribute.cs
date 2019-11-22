using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof attribute
    /// </summary>
    public class ProofAttribute
    {
        /// <summary>
        /// Gets or sets the sub proof index.
        /// </summary>
        /// <value>
        /// The sub proof index.
        /// </value>
        [JsonProperty("sub_proof_index")]
        public int SubProofIndex { get; set; }

        /// <summary>
        /// Gets or sets the raw value of the attribute.
        /// </summary>
        /// <value>
        /// The raw value of the attribute.
        /// </value>
        [JsonProperty("raw")]
        public string Raw { get; set; }

        /// <summary>
        /// Gets or sets the encoded value of the attribute.
        /// </summary>
        /// <value>
        /// The encoded value of the attribute.
        /// </value>
        [JsonProperty("encoded")]
        public string Encoded { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Raw={(Raw?.Length > 0 ? "[hidden]" : null)}, " +
            $"Encoded={(Encoded?.Length > 0 ? "[hidden]" : null)}";
    }
}
