﻿using Hyperledger.Aries.Features.PresentProof;
using Newtonsoft.Json;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Represents a credential object as stored in the wallet.
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Gets or sets the credential object info.
        /// </summary>
        /// <value>The credential object.</value>
        [JsonProperty("cred_info")]
        public CredentialInfo CredentialInfo { get; set; }

        /// <summary>
        /// Gets or sets the non revocation interval for this credential.
        /// </summary>
        /// <value>The non revocation interval.</value>
        [JsonProperty("interval")]
        public RevocationInterval NonRevocationInterval { get; set; }
    }
}