using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.Attachments
{
    /// <summary>
    /// Attachment content
    /// </summary>
    public class AttachmentContent
    {
        /// <summary>
        /// Gets or sets the base64.
        /// </summary>
        /// <value>
        /// The base64.
        /// </value>
        [JsonProperty("base64", NullValueHandling = NullValueHandling.Ignore)]
        public string Base64 { get; set; }

        /// <summary>
        /// Gets or sets the sha26.
        /// </summary>
        /// <value>
        /// The sha26.
        /// </value>
        [JsonProperty("sha256", NullValueHandling = NullValueHandling.Ignore)]
        public string Sha256 { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Links { get; set; }
        
        /// <summary>
        /// Get or sets a JSON web signature for the given data
        /// </summary>
        /// <value>
        /// The JWS object.
        /// </value>
        [JsonProperty("jws", NullValueHandling = NullValueHandling.Ignore)]
        public JsonWebSignature JsonWebSignature { get; set; }
    }
}
