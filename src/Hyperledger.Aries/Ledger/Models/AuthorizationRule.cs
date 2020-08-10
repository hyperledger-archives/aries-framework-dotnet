using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Ledger
{
    /// <summary>
    /// Authorization rule
    /// </summary>
    public class AuthorizationRule
    {
        /// <summary>
        /// Gets or sets the type of the transaction.
        /// </summary>
        /// <value>
        /// The type of the transaction.
        /// </value>
        [JsonProperty("auth_type")]
        [JsonPropertyName("auth_type")]
        public string TransactionType { get; set; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        [JsonProperty("new_value")]
        [JsonPropertyName("new_value")]
        public string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        [JsonProperty("old_value")]
        [JsonPropertyName("old_value")]
        public string OldValue { get; set; }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [JsonProperty("field")]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        [JsonProperty("auth_action")]
        [JsonPropertyName("auth_action")]
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the constraint.
        /// </summary>
        /// <value>
        /// The constraint.
        /// </value>
        [JsonProperty("constraint")]
        public AuthorizationConstraint Constraint { get; set; }
    }
}
