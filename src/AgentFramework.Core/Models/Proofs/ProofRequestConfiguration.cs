using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Proofs
{
    /// <summary>
    /// Proof request configuration.
    /// </summary>
    public class ProofRequestConfiguration
    {
        /// <summary>
        /// Gets or sets the requested attributes.
        /// </summary>
        /// <value>The requested attributes.</value>
        [JsonProperty("requested_attributes")]
        public Dictionary<string, ProofAttributeInfo> RequestedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the requested predicates.
        /// </summary>
        /// <value>The requested predicates.</value>
        [JsonProperty("requested_predicates", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, ProofPredicateInfo> RequestedPredicates { get; set; } =
            new Dictionary<string, ProofPredicateInfo>();

        /// <summary>
        /// Gets or sets the non revoked.
        /// </summary>
        /// <value>The non revoked.</value>
        [JsonProperty("non_revoked", NullValueHandling = NullValueHandling.Ignore)]
        public RevocationInterval NonRevoked { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"RequestedAttributes={string.Join(",", RequestedAttributes ?? new Dictionary<string, ProofAttributeInfo>())}, " +
            $"RequestedPredicates={string.Join(",", RequestedPredicates ?? new Dictionary<string, ProofPredicateInfo>())}, " +
            $"NonRevoked={NonRevoked}";
    }
}