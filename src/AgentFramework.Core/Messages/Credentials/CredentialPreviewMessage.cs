using System;
using System.Collections.Generic;
using AgentFramework.Core.Models.Credentials;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgentFramework.Core.Messages.Credentials
{
    /// <summary>
    /// Represents credential preview message
    /// </summary>
    /// <seealso cref="AgentFramework.Core.Messages.AgentMessage" />
    public class CredentialPreviewMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialPreviewMessage"/> class.
        /// </summary>
        public CredentialPreviewMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.CredentialPreview;
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        [JsonProperty("@context")]
        public JObject Context { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        [JsonProperty("attributes")]
        public IEnumerable<CredentialPreviewAttribute> Attributes { get; set; }
    }
}