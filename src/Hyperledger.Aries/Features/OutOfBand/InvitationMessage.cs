using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.OutOfBand
{
    /// <summary>
    /// Invitation message defined by Aries RFC-0434
    /// </summary>
    public class InvitationMessage : AgentMessage
    {
        /// <summary>
        /// Initialise a new instance of <see cref="InvitationMessage"/>
        /// </summary>
        public InvitationMessage()
        {
            Id = Guid.NewGuid().ToString().ToLowerInvariant();
            Type = MessageTypesHttps.OutOfBand.Invitation;
            UseMessageTypesHttps = true;
        }
        
        /// <summary>
        /// A self-attested string that the sender may want to display to the user
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
        
        /// <summary>
        /// A self-attested code the sender may want to display to the user
        /// </summary>
        [JsonProperty("goal_code", NullValueHandling = NullValueHandling.Ignore)]
        public string GoalCode { get; set; }
        
        /// <summary>
        /// A self-attested string that the sender may want to display to the user about the context-specific goal of the out-of-band message
        /// </summary>
        [JsonProperty("goal", NullValueHandling = NullValueHandling.Ignore)]
        public string Goal { get; set; }
        
        /// <summary>
        /// An array of media (aka mime) types in the order of preference of the sender that the receiver can use in responding to the message
        /// </summary>
        [JsonProperty("accept", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Accept { get; set; }
        
        /// <summary>
        /// An array of protocols in the order of preference of the sender that the receiver can use in responding to the message in order to create or reuse a connection with the sender
        /// </summary>
        [JsonProperty("handshake_protocols", NullValueHandling = NullValueHandling.Ignore)]
        public string[] HandshakeProtocols { get; set; }
        
        /// <summary>
        /// An attachment decorator containing an array of request messages in order of preference that the receiver can using in responding to the message
        /// </summary>
        [JsonProperty("requests~attach", NullValueHandling = NullValueHandling.Ignore)]
        public Attachment[] AttachedRequests { get; set; }
        
        /// <summary>
        /// An array of union types that the receiver uses when responding to the message - either service object or a DID
        /// </summary>
        [JsonProperty("services")]
        public object[] Services { get; set; }
    }
}
