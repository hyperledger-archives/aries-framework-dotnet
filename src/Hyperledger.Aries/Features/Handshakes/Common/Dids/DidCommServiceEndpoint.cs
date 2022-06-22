using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.Handshakes.Common.Dids
{
    public class DidCommServiceEndpoint : IDidDocServiceEndpoint
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("type")] 
        public string Type => DidDocServiceEndpointTypes.DidCommunication;
        
        [JsonProperty("priority")]
        public int Priority { get; set; }
        
        [JsonProperty("recipientKeys")]
        public IList<string> RecipientKeys { get; set; }

        [JsonProperty("routingKeys")]
        public IList<string> RoutingKeys { get; set; }
        
        [JsonProperty("accept")]
        public IList<string> Accept { get; set; }
        
        [JsonProperty("serviceEndpoint")]
        public string ServiceEndpoint { get; set; }
    }
}
