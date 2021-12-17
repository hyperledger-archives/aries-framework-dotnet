using System.Collections.Generic;

namespace Hyperledger.Aries.Features.IssueCredential
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

        /// <summary>
        /// Indicates if a connectionless offer should use did:key format
        /// </summary>
        public bool UseDidKeyFormat { get; set; } = false;
    }
}
