using System;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.Credentials
{
    /// <summary>
    /// A credential content message.
    /// </summary>
    public class CredentialMessage : AgentMessage
    {
        /// <inheritdoc />
        public CredentialMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.Credential;
        }
        
        /// <summary>
        /// Gets or sets the credential json.
        /// </summary>
        /// <value>
        /// The credential json.
        /// </value>
        [JsonProperty("issue")]
        public string CredentialJson { get; set; }

        /// <summary>
        /// Gets or sets the revocation registry identifier.
        /// </summary>
        /// <value>
        /// The revocation registry identifier.
        /// </value>
        [JsonProperty("rev_reg_id")]
        public string RevocationRegistryId { get; set; }

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"Id={Id}, " +
            $"Type={Type}, " +
            $"CredentialJson={(CredentialJson?.Length > 0 ? "[hidden]" : null)}, " +
            $"RevocationRegistryId={RevocationRegistryId}";
    }
}
