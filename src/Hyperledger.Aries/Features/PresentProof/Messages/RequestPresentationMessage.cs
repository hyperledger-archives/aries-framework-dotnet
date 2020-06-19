using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Request presentation message
    /// </summary>
    public class RequestPresentationMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPresentationMessage" /> class.
        /// </summary>
        public RequestPresentationMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.PresentProofNames.RequestPresentation;
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
        [JsonProperty("request_presentations~attach")]
        public Attachment[] Requests { get; set; }
    }
}