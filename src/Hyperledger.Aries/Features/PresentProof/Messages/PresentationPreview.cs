using System;
using Hyperledger.Aries.Agents;

using Hyperledger.Aries.Decorators.Attachments;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Presentation Preview Inner Message
    /// </summary>
    public class PresentationPreviewMessage : AgentMessage
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="PresentationPreviewMessage" /> class.
		/// </summary>
        public PresentationPreviewMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypes.PresentProofNames.PresentationPreview;
        }


        /// <summary>
        /// Gets or sets the attribute previews for the PresentationPreviewMessage.
        /// </summary>
        /// <value>
        /// The proposed attributes.
        /// </value>
        [JsonProperty("attributes", NullValueHandling = NullValueHandling.Ignore)]
        public ProposedAttribute[] ProposedAttributes { get; set; }

        /// <summary>
        /// Gets or sets the predicate previews
        /// </summary>
        /// <value>The proposed predicates</value>
        [JsonProperty("predicates")]
        public ProposedPredicate[] ProposedPredicates { get; set; }

    }

    public class ProposedAttribute
    {

        /// <summary>
        /// Gets or sets the Name of the Attribute
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the credential definition Id of the attribute
        /// </summary>
        [JsonProperty("cred_def_id")]
        public string CredentialDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the schema id of the attribute
        /// </summary>
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the issuer dod of the attribute
        /// </summary>
        [JsonProperty("issuer_did")]
        public string IssuerDid { get; set; }

        /// <summary>
        /// Gets or sets the mime-type
        /// </summary>
        [JsonProperty("mime-type")]
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the attribute value
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the reference
        /// </summary>
        [JsonProperty("referent")]
        public string Referent { get; set; }

    }

    public class ProposedPredicate
    {
        /// <summary>
        /// Gets or sets the name of the Attribute
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the credential definition Id of the attribute
        /// </summary>
        [JsonProperty("cred_def_id")]
        public string CredentialDefinitionId { get; set; }

        /// <summary>
        /// Gets or sets the issuer did of the attribute
        /// </summary>
        [JsonProperty("issuer_did")]
        public string IssuerDid { get; set; }

        /// <summary>
        /// Gets or sets the schema Id of the attribute
        /// </summary>
        [JsonProperty("schema_id")]
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the predicate operator (>, >=, <, <=)
        /// </summary>
        [JsonProperty("predicate")]
        public string Predicate { get; set; }

        /// <summary>
        /// Gets or sets the predicate threadshold
        /// </summary>
        [JsonProperty("threshold")]
        public int Threshold { get; set; }

        /// <summary>
        /// Gets or sets the referent
        /// </summary>
        [JsonProperty("referent")]
        public string Referent { get; set; }
    }
}


