using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Features.DidExchange
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
        [JsonPropertyName("DID")]
        public string Did { get; set; }

        /// <summary>
        /// Gets or sets the did doc.
        /// </summary>
        /// <value>
        /// The did doc.
        /// </value>
        [JsonProperty("DIDDoc")]
        [JsonPropertyName("DIDDoc")]
        public DidDoc DidDoc { get; set; }
    }
}
