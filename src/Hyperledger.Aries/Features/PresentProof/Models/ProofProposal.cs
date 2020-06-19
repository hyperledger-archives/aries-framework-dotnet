using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof proposal
    /// </summary>
    public class ProofProposal
    {
        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The Comment.</value>
        [JsonProperty("comment")]
        public string Comment { get; set; }


      
        /// <summary>
        /// Gets or sets the proposed attributes.
        /// </summary>
        /// <value>The proposed attributes.</value>
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProposedAttribute> ProposedAttributes { get; set; } = new List<ProposedAttribute>();

        /// <summary>
        /// Gets or sets the proposed predicates.
        /// </summary>
        /// <value>The proposed predicates.</value>
        [JsonProperty("predicates", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProposedPredicate> ProposedPredicates { get; set; } = new List<ProposedPredicate>();
    
    }
}