using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Represents credential preview attribute
    /// </summary>
    [Newtonsoft.Json.JsonConverter(typeof(CredentialPreviewAttributeConverter))]
    public class CredentialPreviewAttribute
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public CredentialPreviewAttribute() => MimeType = CredentialMimeTypes.TextMimeType;

        /// <summary>
        /// String type credential attribute constructor.
        /// </summary>
        /// <param name="name">Name of the credential attribute.</param>
        /// <param name="value">Value of the credential attribute.</param>
        public CredentialPreviewAttribute(string name, string value) : this()
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>
        /// The type of the MIME.
        /// </value>
        [JsonProperty("mime-type")]
        [JsonPropertyName("mime-type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}