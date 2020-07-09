using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Requested credentials dto.
    /// </summary>
    public class RequestedCredentials
    {
        /// <summary>
        /// Gets or sets the requested attributes.
        /// </summary>
        /// <value>The requested attributes.</value>
        [JsonProperty("requested_attributes")]
        [JsonPropertyName("requested_attributes")]
        public Dictionary<string, RequestedAttribute> RequestedAttributes { get; set; } =
            new Dictionary<string, RequestedAttribute>();

        /// <summary>
        /// Gets or sets the self attested attributes.
        /// </summary>
        /// <value>The self attested attributes.</value>
        [JsonProperty("self_attested_attributes")]
        [JsonPropertyName("self_attested_attributes")]
        public Dictionary<string, string> SelfAttestedAttributes { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the requested predicates.
        /// </summary>
        /// <value>The requested predicates.</value>
        [JsonProperty("requested_predicates")]
        [JsonPropertyName("requested_predicates")]
        public Dictionary<string, RequestedAttribute> RequestedPredicates { get; set; }
            = new Dictionary<string, RequestedAttribute>();


        /// <summary>
        /// Gets a collection of distinct credential identifiers found in this object.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<string> GetCredentialIdentifiers()
        {
            var credIds = new List<string>();
            credIds.AddRange(RequestedAttributes.Values.Select(x => x.CredentialId));
            credIds.AddRange(RequestedPredicates.Values.Select(x => x.CredentialId));
            return credIds.Distinct();
        }
    }
}