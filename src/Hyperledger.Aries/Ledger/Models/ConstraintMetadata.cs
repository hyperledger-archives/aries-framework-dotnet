using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Ledger
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
        [JsonPropertyName("fees")]
        public string Fee { get; set; }
    }
}
