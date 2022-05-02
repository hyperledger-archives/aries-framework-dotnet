using System;
using System.Text.Json.Serialization;
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
        /// Initializes a new instance of the <see cref="PresentationMessage" /> class.
        /// </summary>
        public PresentationMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.PresentProofNames.Presentation : MessageTypes.PresentProofNames.Presentation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationMessage" /> class.
        /// </summary>
        public PresentationMessage(bool useMessageTypesHttps = false) : base(useMessageTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps ? MessageTypesHttps.PresentProofNames.Presentation : MessageTypes.PresentProofNames.Presentation;
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
        [JsonPropertyName("presentations~attach")]
        public Attachment[] Presentations { get; set; }
    }
}
