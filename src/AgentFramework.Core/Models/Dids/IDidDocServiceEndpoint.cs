using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Dids
{
    /// <summary>
    /// DID doc service interface.
    /// </summary>
    public interface IDidDocServiceEndpoint
    {
        /// <summary>
        /// Id of the service.
        /// </summary>
        [JsonProperty("id")]
        string Id { get; set; }

        /// <summary>
        /// Type of the service.
        /// </summary>
        [JsonProperty("type")]
        string Type { get; }

        /// <summary>
        /// Endpoint of the service.
        /// </summary>
        [JsonProperty("serviceEndpoint")]
        string ServiceEndpoint { get; set; }
    }
}