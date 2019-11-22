using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof Presentation message
    /// </summary>
    public class PresentationMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instace of the <see cref="PresentationMessage" /> class.
        /// </summary>
        public PresentationMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.PresentProofNames.Presentation;
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
        /// Gets or sets the request presentation attachments
        /// </summary>
        /// <value></value>
        [JsonProperty("presentations~attach")]
        public Attachment[] Presentations { get; set; }
    }
}