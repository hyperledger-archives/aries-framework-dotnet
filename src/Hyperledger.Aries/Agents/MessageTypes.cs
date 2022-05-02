namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Protocol message types
    /// </summary>
    public static class MessageTypes
    {
        /// <summary>
        /// The connection acknowledgement.
        /// </summary>
        public const string ConnectionAcknowledgement = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/connections/1.0/ack";

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
        public const string DiscoveryQueryMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/discover-features/1.0/query";

        /// <summary>
        /// Discovery Disclose Message Type.
        /// </summary>
        public const string DiscoveryDiscloseMessageType = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/discover-features/1.0/disclose";

        /// <summary>
        /// Issue Credential
        /// </summary>
        public static class IssueCredentialNames
        {
            /// <summary>
            /// Credential Acknowledge Message Type Name
            /// </summary>
            public const string AcknowledgeCredential = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/issue-credential/1.0/ack";
            
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

        /// <summary>
        /// Type names for Present Proof Protocol
        /// </summary>
        public class PresentProofNames
        {
            /// <summary>
            /// Acknowledge Presentation Message Type Name
            /// </summary>
            public const string AcknowledgePresentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/ack";
            
            /// <summary>
            /// Propose Presentation Message Type Name
            /// </summary>
            public const string ProposePresentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/propose-presentation";

            /// <summary>
            /// Request Presentation Message Type Name
            /// </summary>
            public const string RequestPresentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/request-presentation";

            /// <summary>
            /// Presentation Message Type Name
            /// </summary>
            public const string Presentation = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/presentation";

            /// <summary>
            /// Presentation Preview Message Type Name
            /// </summary>
            public const string PresentationPreview = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/present-proof/1.0/presentation-preview";
        }
    }
}
