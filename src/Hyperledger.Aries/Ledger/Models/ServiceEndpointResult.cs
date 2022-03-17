using Newtonsoft.Json;

namespace Hyperledger.Aries.Ledger.Models
{
    /// <summary>
    /// Service endpoint result
    /// </summary>
    public class ServiceEndpointResult
    {
        [JsonProperty("endpoint")]
        public ServiceEndpoint Result { get; set; }
        
        /// <summary>
        /// Service endpoint
        /// </summary>
        public class ServiceEndpoint
        {
            [JsonProperty("endpoint")]
            public string Endpoint { get; set; }
        }
    }
}
