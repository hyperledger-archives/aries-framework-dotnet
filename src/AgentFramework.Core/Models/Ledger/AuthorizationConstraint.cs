using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Ledger
{
    /// <summary>
    /// Authorization constraint
    /// </summary>
    public class AuthorizationConstraint
    {
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [JsonProperty("role")]
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transaction writer must be owner.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [must be owner]; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty("need_to_be_owner")]
        public bool MustBeOwner { get; set; }

        /// <summary>
        /// Gets or sets the signature count.
        /// </summary>
        /// <value>
        /// The signature count.
        /// </value>
        [JsonProperty("sig_count")]
        public int SignatureCount { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>
        /// The metadata.
        /// </value>
        [JsonProperty("metadata")]
        public ConstraintMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the constraint identifier.
        /// </summary>
        /// <value>
        /// The constraint identifier.
        /// </value>
        [JsonProperty("constraint_id")]
        public string ConstraintId { get; set; }

        /// <summary>
        /// Gets or sets the constraints.
        /// </summary>
        /// <value>
        /// The constraints.
        /// </value>
        [JsonProperty("auth_constraints")]
        public IList<AuthorizationConstraint> Constraints { get; set; }
    }
}
