using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials
{
    /// <summary>
    /// A credential request content message.
    /// </summary>
    public class CredentialRequestMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialRequestMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.CredentialRequest;
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>
        /// The comment.
        /// </value>
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the credential request json.
        /// </summary>
        /// <value>
        /// The credential request json.
        /// </value>
        [JsonProperty("request")]
        public string CredentialRequestJson { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, " +
            $"CredentialRequestJson={(CredentialRequestJson?.Length > 0 ? "[hidden]" : null)}";
    }
}
