using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages.EphemeralChallenge
{
    /// <inheritdoc />
    /// <summary>
    /// Represents an ephemeral challenge message.
    /// </summary>
    public class EphemeralChallengeMessage : AgentMessage
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EphemeralChallengeMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.EphemeralChallenge;
        }

        /// <summary>
        /// Challengers name.
        /// </summary>
        [JsonProperty("challengerName")]
        public string ChallengerName { get; set; }

        /// <summary>
        /// Challengers name.
        /// </summary>
        [JsonProperty("challengerImageUrl")]
        public string ChallengerImageUrl { get; set; }

        /// <summary>
        /// Array of recipient keys.
        /// </summary>
        [JsonProperty("recipientKeys")]
        public IList<string> RecipientKeys { get; set; }

        /// <summary>
        /// Array or routing keys.
        /// </summary>
        [JsonProperty("routingKeys")]
        public IList<string> RoutingKeys { get; set; }

        /// <summary>
        /// Service endpoint.
        /// </summary>
        [JsonProperty("serviceEndpoint")]
        public string ServiceEndpoint { get; set; }

        /// <summary>
        /// The challenge.
        /// </summary>
        [JsonProperty("challenge")]
        public Models.EphemeralChallenge.EphemeralChallengeContents Challenge { get; set; }
    }
}
