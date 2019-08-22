using Newtonsoft.Json;

namespace AgentFramework.Core.Models.Credentials
{
    /// <summary>
    /// Represents credential preview attribute
    /// </summary>
    [JsonConverter(typeof(CredentialPreviewAttributeConverter))]
    public class CredentialPreviewAttribute
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public CredentialPreviewAttribute() { }

        /// <summary>
        /// String type credential attribute constructor.
        /// </summary>
        /// <param name="name">Name of the credential attribute.</param>
        /// <param name="value">Value of the credential attribute.</param>
        public CredentialPreviewAttribute(string name, string value)
        {
            Name = name;
            Value = value;
            MimeType = CredentialMimeTypes.TextMimeType;
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
        [JsonProperty("mime_type")]
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