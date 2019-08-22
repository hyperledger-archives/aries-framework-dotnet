using System.Collections.Generic;

namespace AgentFramework.Core.Models.Credentials
{
    /// <summary>
    /// Config for controlling credential offer creation.
    /// </summary>
    public class OfferConfiguration
    {
        /// <summary>
        /// Id of the credential definition used to create
        /// the credential offer.
        /// </summary>
        public string CredentialDefinitionId { get; set; }

        /// <summary>
        /// Did of the issuer generating the offer.
        /// </summary>
        public string IssuerDid { get; set; }

        /// <summary>
        /// [Optional] For setting the credential values at the offer stage.
        /// Note these attributes are not disclosed in the
        /// offer.
        /// </summary>
        public IEnumerable<CredentialPreviewAttribute> CredentialAttributeValues { get; set; }

        /// <summary>
        /// Controls the tags that are persisted against the offer record.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; }
        
        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"CredentialDefinitionId={CredentialDefinitionId}, " +
            $"IssuerDid={IssuerDid}, " +
            $"CredentialAttributeValues={string.Join(",", CredentialAttributeValues ?? new List<CredentialPreviewAttribute>())}, " +
            $"Tags={string.Join(",", Tags)}";
    }
}
