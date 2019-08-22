using AgentFramework.Core.Models.Dids;
using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Connections
{
    /// <summary>
    /// Connection object
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Gets or sets the did.
        /// </summary>
        /// <value>
        /// The did.
        /// </value>
        [JsonProperty("DID")]
        public string Did { get; set; }

        /// <summary>
        /// Gets or sets the did doc.
        /// </summary>
        /// <value>
        /// The did doc.
        /// </value>
        [JsonProperty("DIDDoc")]
        public DidDoc DidDoc { get; set; }
    }
}
