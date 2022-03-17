using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange.Models
{
    public class DidExchangeRequestMessage : AgentMessage
    {
        public DidExchangeRequestMessage() : base(true)
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.DidExchange.Request;
        }
        
        [JsonProperty("label")]
        public string Label { get; set; }
        
        [JsonProperty("did")]
        public string Did { get; set; }
        
        [JsonProperty("did_doc~attach", NullValueHandling = NullValueHandling.Ignore)]
        public Attachment DidDoc { get; set; }
    }
}
