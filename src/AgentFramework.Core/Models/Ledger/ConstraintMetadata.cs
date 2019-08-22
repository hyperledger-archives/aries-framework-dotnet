using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Ledger
{
    /// <summary>
    /// Constraint metadata
    /// </summary>
    public class ConstraintMetadata
    {
        /// <summary>
        /// Gets or sets the fee.
        /// </summary>
        /// <value>
        /// The fee.
        /// </value>
        [JsonProperty("fees")]
        public string Fee { get; set; }
    }
}
