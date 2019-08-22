using System.Collections.Generic;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Dids
{
    /// <summary>
    /// Indy Agent Did Doc Service.
    /// </summary>
    public class IndyAgentDidDocService : IDidDocServiceEndpoint
    {
        /// <inheritdoc />
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <inheritdoc />
        [JsonProperty("type")]
        public string Type => DidDocServiceEndpointTypes.IndyAgent;

        /// <summary>
        /// Array of recipient key references.
        /// </summary>
        [JsonProperty("recipientKeys")]
        public IList<string> RecipientKeys { get; set; }

        /// <summary>
        /// Array or routing key references.
        /// </summary>
        [JsonProperty("routingKeys")]
        public IList<string> RoutingKeys { get; set; }

        /// <summary>
        /// Service endpoint.
        /// </summary>
        [JsonProperty("serviceEndpoint")]
        public string ServiceEndpoint { get; set; }
    }
}
