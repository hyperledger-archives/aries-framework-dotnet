using Hyperledger.Aries.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Credential definition configuration
    /// </summary>
    public class CredentialDefinitionConfiguration
    {
        /// <summary>
        /// Gets or sets the schema identifier.
        /// </summary>
        /// <value>
        /// The schema identifier.
        /// </value>
        public string SchemaId { get; set; }

        /// <summary>
        /// Gets or sets the issuer DID. If this field is not specified, it will
        /// be read from the <see cref="ProvisioningRecord" />.
        /// </summary>
        /// <value>
        /// The issuer did.
        /// </value>
        public string IssuerDid { get; set; }

        /// <summary>
        /// Gets or sets the tag of the credential definition.
        /// Tags allow differentiating credential definitions of the same schema and issuer.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public string Tag { get; set; } = "default";

        /// <summary>
        /// Gets or sets the enable revocation.
        /// </summary>
        /// <value>
        /// The enable revocation.
        /// </value>
        public bool EnableRevocation { get; set; }

        /// <summary>
        /// Gets or sets the size of the revocation registry.
        /// </summary>
        /// <value>
        /// The size of the revocation registry.
        /// </value>
        public int RevocationRegistrySize { get; set; } = 1024;

        /// <summary>
        /// Gets or sets a value indicating whether the revocation registry scales automatically.
        /// This means that a new revocation registry will be created as soon as the existing one
        /// reaches maximum credential issuance.
        /// This feature improves the size of the revocation registry tails file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [revocation registry automatic scale]; otherwise, <c>false</c>.
        /// </value>
        public bool RevocationRegistryAutoScale { get; set; } = true;

        /// <summary>
        /// Gets the revocation registry base URI. If this is not specified, the value
        /// will be taken from <see cref="AgentOptions" />
        /// </summary>
        /// <value>
        /// The revocation registry base URI.
        /// </value>
        public string RevocationRegistryBaseUri { get; set; }
    }
}
