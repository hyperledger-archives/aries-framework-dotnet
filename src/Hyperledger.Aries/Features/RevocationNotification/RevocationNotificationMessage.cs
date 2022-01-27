using System;
using Hyperledger.Aries.Agents;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <summary>
    ///  A message representing a Revocation Notification Message according to v.1.0
    /// </summary>
    public class RevocationNotificationMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RevocationNotificationMessage"/> class.
        /// </summary>
        public RevocationNotificationMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps
                ? MessageTypesHttps.IssueCredentialNames.RevocationNotification
                : MessageTypes.IssueCredentialNames.RevocationNotification;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RevocationNotificationMessage"/> class.
        /// </summary>
        public RevocationNotificationMessage(bool useMessageTypeHttps = false) : base(useMessageTypeHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps
                ? MessageTypesHttps.IssueCredentialNames.RevocationNotification
                : MessageTypes.IssueCredentialNames.RevocationNotification;
        }

        /// <summary>
        /// The thread ID of the issue-credential-v2 protocol which was used to issue one or more credentials
        /// that have been revoked by the issuer. If multiple credentials were issued, each credential has
        /// a different credential format but contains the same claims as described here; therefore, this message
        /// notifies the holder that all of these credentials have been revoked.
        /// </summary>
        /// <value>The thread id</Value>
        public string ThreadId { get; set; }

        /// <summary>
        /// A field that provides some human readable information about the revocation notification.
        /// This is typically the reason for the revocation as deemed appropriate by the issuer.
        /// </summary>
        /// <value>The comment</value>
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
