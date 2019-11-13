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

        /// <summary>
        /// Issue Credential
        /// </summary>
        public static class IssueCredentialNames
        {
            /// <summary>
            /// Credential Propose Message Type Name
            /// </summary>
            public const string ProposeCredential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/propose-credential";

            /// <summary>
            /// Credential Preview Message Type Name
            /// </summary>
            public const string PreviewCredential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/credential-preview";

            /// <summary>
            /// Offer Credential Message Type Name
            /// </summary>
            public const string OfferCredential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/offer-credential";

            /// <summary>
            /// Request Credential Message Type Name
            /// </summary>
            public const string RequestCredential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/request-credential";

            /// <summary>
            /// Issue Credential Message Type Name
            /// </summary>
            public const string IssueCredential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/issue-credential";
        }

        public class PresentProofNames
        {
            public const string ProposePresentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/propose-presentation";
            public const string RequestPresentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/request-presentation";
            public const string Presentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/presentation";
            public const string PresentationPreview = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/presentation-preview";
        }
    }
}