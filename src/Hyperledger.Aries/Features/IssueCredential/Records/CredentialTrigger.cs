namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Enumeration of possible triggers that change the credentials state
    /// </summary>
    public enum CredentialTrigger
    {
        /// <summary>
        /// The request
        /// </summary>
        Request,
        /// <summary>
        /// The issue
        /// </summary>
        Issue,
        /// <summary>
        /// The reject
        /// </summary>
        Reject,
        /// <summary>
        /// The revoke
        /// </summary>
        Revoke
    }
}
