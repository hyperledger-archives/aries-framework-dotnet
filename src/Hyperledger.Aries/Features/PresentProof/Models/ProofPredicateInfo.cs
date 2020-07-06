using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <inheritdoc />
    public class ProofPredicateInfo : ProofAttributeInfo
    {
        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        [JsonProperty("p_type")]
        [JsonPropertyName("p_type")]
        public string PredicateType { get; set; }

        /// <summary>
        /// Gets or sets the predicate value.
        /// </summary>
        /// <value>The predicate value.</value>
        [JsonProperty("p_value")]
        [JsonPropertyName("p_value")]
        public int PredicateValue { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"PredicateType={PredicateType}, " +
            $"PredicateValue={PredicateValue}";
    }
}