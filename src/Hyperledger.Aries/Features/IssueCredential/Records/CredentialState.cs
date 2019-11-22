using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Enumeration of possible credential states
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CredentialState
    {
        /// <summary>
        /// The offered
        /// </summary>
        Offered = 0,
        /// <summary>
        /// The requested
        /// </summary>
        Requested,
        /// <summary>
        /// The issued
        /// </summary>
        Issued,
        /// <summary>
        /// The rejected
        /// </summary>
        Rejected,
        /// <summary>
        /// The revoked
        /// </summary>
        Revoked
    }
}
