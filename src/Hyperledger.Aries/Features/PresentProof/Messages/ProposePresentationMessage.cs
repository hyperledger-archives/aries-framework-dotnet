using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof.Messages
{
    /// <summary>
    /// Propose Presentation Message
    /// </summary>
    public class ProposePresentationMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProposePresentationMessage"/> class.
        /// </summary>
        public ProposePresentationMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.PresentProofNames.ProposePresentation;
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
        /// Gets or sets the presentation proposal
        /// </summary>
        /// <value></value>
        [JsonProperty("presentation_proposal")]
        public PresentationPreview PresentationPreview { get; set; }

    }
}
