using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.Attachments
{
    /// <summary>
    /// Attach decorator
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// Gets or sets the ID
        /// </summary>
        /// <value></value>
        [JsonProperty("@id")]
        [JsonPropertyName("@id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>
        /// The nickname.
        /// </value>
        [JsonProperty("nickname", NullValueHandling = NullValueHandling.Ignore)]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>
        /// The type of the MIME.
        /// </value>
        [JsonProperty("mime-type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("mime-type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        [JsonProperty("filename", NullValueHandling = NullValueHandling.Ignore)]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the last modified time.
        /// </summary>
        /// <value>
        /// The last modified time.
        /// </value>
        [JsonProperty("lastmod_time", NullValueHandling = NullValueHandling.Ignore)]
        [JsonPropertyName("lastmod_time")]
        public DateTimeOffset? LastModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the attachment data
        /// </summary>
        /// <value></value>
        [JsonProperty("data")]
        public AttachmentContent Data { get; set; }
    }
}
