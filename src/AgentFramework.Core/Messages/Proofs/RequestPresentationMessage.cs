using System;
using AgentFramework.Core.Decorators.Attachments;
using Newtonsoft.Json;

namespace AgentFramework.Core.Messages
{
    /// <summary>
    /// Request presentation message
    /// </summary>
    public class RequestPresentationMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instace of the <see cref="RequestPresentationMessage" /> class.
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