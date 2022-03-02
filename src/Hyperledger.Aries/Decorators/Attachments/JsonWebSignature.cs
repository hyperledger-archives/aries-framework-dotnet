using Hyperledger.Aries.Extensions;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.Attachments
{
    public class JsonWebSignature
    {
        /// <summary>
        /// Json Web Signature Header
        /// </summary>
        [JsonProperty("header")]
        public JsonWebSignatureHeader Header { get; set; }
        
        /// <summary> Json Web Signature Protected Header </summary>
        /// <value> Base64 encoded protected header </value>
        [JsonProperty("protected")]
        public string Protected { get; set; }
        
        /// <summary> Json Web Signature </summary>
        /// <value>Base64 encoded signature</value>
        [JsonProperty("signature")]
        public string Signature { get; set; }

        public override string ToString() => this.ToJson();
    }

    public class JsonWebSignatureHeader
    {
        /// <summary> Json Web Signature Kid </summary>
        [JsonProperty("kid")]
        public string Kid { get; set; }
    }
}
