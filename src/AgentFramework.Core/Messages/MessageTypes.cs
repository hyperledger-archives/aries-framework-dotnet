namespace AgentFramework.Core.Messages
{
    /// <summary>
    /// Protocol message types
    /// </summary>
    public static class MessageTypes
    {
        /// <summary>
        /// The connection invitation.
        /// </summary>
        public const string ConnectionInvitation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/invitation";

        /// <summary>
        /// The connection request.
        /// </summary>
        public const string ConnectionRequest = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/request";

        /// <summary>
        /// The connection response.
        /// </summary>
        public const string ConnectionResponse = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/response";

        /// <summary>
        /// The credential offer.
        /// </summary>
        public const string CredentialOffer = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-issuance/0.1/credential-offer";

        /// <summary>
        /// The credential preview
        /// </summary>
        public const string CredentialPreview = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-issuance/0.1/credential-preview";

        /// <summary>
        /// The credential request.
        /// </summary>
        public const string CredentialRequest = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-issuance/0.1/credential-request";

        /// <summary>
        /// The credential.
        /// </summary>
        public const string Credential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-issuance/0.1/credential-issue";

        /// <summary>
        /// The proof request.
        /// </summary>
        public const string ProofRequest = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-presentation/0.1/presentation-request";

        /// <summary>
        /// The disclosed proof.
        /// </summary>
        public const string DisclosedProof = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/credential-presentation/0.1/credential-presentation";

        /// <summary>
        /// Ephemeral Challenge Message
        /// </summary>
        public const string EphemeralChallenge = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/ephemeral_challenge/1.0/challenge";

        /// <summary>
        /// The ephemeral challenge response.
        /// </summary>
        public const string EphemeralChallengeResponse = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/ephemeral_challenge/1.0/challenge_response";

        /// <summary>
        /// Routing Messages
        /// </summary>
        public const string Forward = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/routing/1.0/forward";

        /// <summary>
        /// Basic Message Type.
        /// </summary>
        public const string BasicMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basicmessage/1.0/message";

        /// <summary>
        /// Ping Message Type.
        /// </summary>
        public const string TrustPingMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/trust_ping/1.0/ping";

        /// <summary>
        /// Ping Response Message Type.
        /// </summary>
        public const string TrustPingResponseMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/trust_ping/1.0/ping_response";

        /// <summary>
        /// Discovery Query Message Type.
        /// </summary>
        public const string DiscoveryQueryMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/protocol-discovery/1.0/query";

        /// <summary>
        /// Discovery Disclose Message Type.
        /// </summary>
        public const string DiscoveryDiscloseMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/protocol-discovery/1.0/disclose";
    }
}