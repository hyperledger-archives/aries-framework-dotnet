using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Attachments
{
    /// <summary>
    /// 
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
        public string Sha26 { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Links { get; set; }
    }
}