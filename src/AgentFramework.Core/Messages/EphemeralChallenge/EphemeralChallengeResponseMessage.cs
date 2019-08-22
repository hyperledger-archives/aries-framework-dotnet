using System;
using AgentFramework.Core.Models.EphemeralChallenge;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.EphemeralChallenge
{
    /// <summary>
    /// Enumeration of possible challenge response statuses
    /// </summary>
    public enum EphemeralChallengeResponseStatus
    {
        /// <summary>
        /// Indicates an accepted state of an ephemeral challenge.
        /// </summary>
        Accepted,
        /// <summary>
        /// Indicates a rejected state of an ephemeral challenge.
        /// </summary>
        Rejected
    }

    /// <inheritdoc />
    /// <summary>
    /// Represents an ephemeral challenge response message.
    /// </summary>
    public class EphemeralChallengeResponseMessage : AgentMessage
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EphemeralChallengeResponseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.EphemeralChallengeResponse;
        }

        /// <summary>
        /// Challenge status.
        /// </summary>
        [JsonProperty("status")]
        public EphemeralChallengeResponseStatus Status { get; set; }

        /// <summary>
        /// Challenge response.
        /// </summary>
        [JsonProperty("response")]
        public EphemeralChallengeContents Response { get; set; }
    }
}
