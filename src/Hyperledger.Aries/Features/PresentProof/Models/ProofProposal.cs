using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof request.
    /// </summary>
    public class ProofProposal
    {
        // /// <summary>
        // /// Gets or sets the name.
        // /// </summary>
        // /// <value>The name.</value>
        // [JsonProperty("name")]
        // public string Name { get; set; }

        // /// <summary>
        // /// Gets or sets the version.
        // /// </summary>
        // /// <value>The version.</value>
        // [JsonProperty("version")]
        // public string Version { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The Comment.</value>
        [JsonProperty("comment")]
        public string Comment { get; set; }


        // /// <summary>
        // /// Gets or sets the nonce.
        // /// </summary>
        // /// <value>The nonce.</value>
        // [JsonProperty("nonce")]
        // public string Nonce { get; set; }

        /// <summary>
        /// Gets or sets the requested attributes.
        /// </summary>
        /// <value>The requested attributes.</value>
        [JsonProperty("requested_attributes")]
        public List<ProposedAttribute> ProposedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the requested predicates.
        /// </summary>
        /// <value>The requested predicates.</value>
        [JsonProperty("requested_predicates", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProposedPredicate> ProposedPredicates { get; set; }

        //     /// <summary>
        //     /// Gets or sets the non revoked.
        //     /// </summary>
        //     /// <value>The non revoked.</value>
        //     [JsonProperty("non_revoked", NullValueHandling = NullValueHandling.Ignore)]
        //     public RevocationInterval NonRevoked { get; set; }

        //     /// <inheritdoc />
        //     public override string ToString() =>
        //         $"{GetType().Name}: " +
        //         $"Name={Name}, " +
        //         $"Version={Version}, " +
        //         $"Nonce={Nonce}, " +
        //         $"RequestedAttributes={string.Join(",", RequestedAttributes ?? new Dictionary<string, ProofAttributeInfo>())}, " +
        //         $"RequestedPredicates={string.Join(",", RequestedPredicates ?? new Dictionary<string, ProofPredicateInfo>())}, " +
        //         $"NonRevoked={NonRevoked}";
        // }
    }
}