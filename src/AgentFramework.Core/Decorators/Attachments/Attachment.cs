﻿using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Decorators.Attachments
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
        public DateTimeOffset? LastModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        [Obsolete("Please use Data field instead.")]
        public AttachmentContent Content { get; set; }

        /// <summary>
        /// Gets or sets the attachment data
        /// </summary>
        /// <value></value>
        [JsonProperty("data")]
        public AttachmentContent Data { get; set; }
    }
}
