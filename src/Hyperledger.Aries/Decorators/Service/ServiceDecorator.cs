using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.Service
{
    /// <summary>
    /// Service Decorator
    /// 
    /// Based on specification Aries RFC 0056: Service Decorator
    /// https://github.com/hyperledger/aries-rfcs/tree/master/features/0056-service-decorator
    /// </summary>
    public class ServiceDecorator
    {
        /// <summary>
        /// Recipient Keys
        /// </summary>
        /// <value></value>
        [JsonProperty("recipientKeys")]
        public IEnumerable<string> RecipientKeys { get; set; }

        /// <summary>
        /// Routing Keys
        /// </summary>
        /// <value></value>
        [JsonProperty("routingKeys")]
        public IEnumerable<string> RoutingKeys { get; set; }

        /// <summary>
        /// Service endpoint URL
        /// </summary>
        /// <value></value>
        [JsonProperty("serviceEndpoint")]
        public string ServiceEndpoint { get; set; }
    }
}