using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Requested attribute dto.
    /// </summary>
    public class RequestedAttribute
    {
        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        /// <value>The credential identifier.</value>
        [JsonProperty("cred_id")]
        [JsonPropertyName("cred_id")]
        public string CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public long? Timestamp { get; set; }

        /// <summary>Gets or sets a value indicating if this attribute is revealed.</summary>
        /// <value>The revealed.</value>
        [JsonProperty("revealed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Revealed { get; set; }
    }
}