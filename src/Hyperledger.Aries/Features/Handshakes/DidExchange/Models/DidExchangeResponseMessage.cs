using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange.Models
{
    public class DidExchangeResponseMessage : AgentMessage
    {
        public DidExchangeResponseMessage() : base(true)
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.DidExchange.Response;
        }
        
        [JsonProperty("did")]
        public string Did { get; set; }
        
        [JsonProperty("did_doc~attach", NullValueHandling = NullValueHandling.Ignore)]
        public Attachment DidDoc { get; set; }
    }
}
