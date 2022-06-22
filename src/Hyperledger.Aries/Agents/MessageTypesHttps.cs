namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Protocol message types using "https" scheme
    /// </summary>
    public static class MessageTypesHttps
    {
        /// <summary>
        /// The connection acknowledgement.
        /// </summary>
        public const string ConnectionAcknowledgement = "https://didcomm.org/connections/1.0/ack";
        
        /// <summary>
        /// The connection invitation.
        /// </summary>
        public const string ConnectionInvitation = "https://didcomm.org/connections/1.0/invitation";

        /// <summary>
        /// The connection request.
        /// </summary>
        public const string ConnectionRequest = "https://didcomm.org/connections/1.0/request";

        /// <summary>
        /// The connection response.
        /// </summary>
        public const string ConnectionResponse = "https://didcomm.org/connections/1.0/response";

        /// <summary>
        /// Routing Messages
        /// </summary>
        public const string Forward = "https://didcomm.org/routing/1.0/forward";

        /// <summary>
        /// Basic Message Type.
        /// </summary>
        public const string BasicMessageType = "https://didcomm.org/basicmessage/1.0/message";

        /// <summary>
        /// Ping Message Type.
        /// </summary>
        public const string TrustPingMessageType = "https://didcomm.org/trust_ping/1.0/ping";

        /// <summary>
        /// Ping Response Message Type.
        /// </summary>
        public const string TrustPingResponseMessageType = "https://didcomm.org/trust_ping/1.0/ping_response";

        /// <summary>
        /// Discovery Query Message Type.
        /// </summary>
        public const string DiscoveryQueryMessageType = "https://didcomm.org/discover-features/1.0/query";

        /// <summary>
        /// Discovery Disclose Message Type.
        /// </summary>
        public const string DiscoveryDiscloseMessageType = "https://didcomm.org/discover-features/1.0/disclose";
        
        /// <summary>
        /// Out of band message types
        /// </summary>
        public static class OutOfBand
        {
            public const string Invitation = "https://didcomm.org/out-of-band/1.1/invitation";

            public const string HandshakeReuse = "https://didcomm.org/out-of-band/1.1/handshake-reuse";

            public const string HandshakeReuseAccepted = "https://didcomm.org/out-of-band/1.1/handshake-reuse-accepted";

            public const string ProblemReport = "https://didcomm.org/out-of-band/1.1/problem_report";
        }

        /// <summary>
        /// Acknowledge Revocation Notification Message Type.
        /// </summary>
        public const string RevocationNotificationAcknowledgement = "https://didcomm.org/revocation_notification/1.0/ack";
        
        /// <summary>
        /// Revocation Notification Message Type.
        /// </summary>
        public const string RevocationNotification = "https://didcomm.org/revocation_notification/1.0/revoke";
        
        /// <summary>
        /// Did Exchange Message Types
        /// </summary>
        public static class DidExchange
        {
            /// <summary>
            ///  DidExchange Request Message Type
            /// </summary>
            public const string Request = "https://didcomm.org/didexchange/1.0/request";
        
            /// <summary>
            /// DidExchange Response Message Type
            /// </summary>
            public const string Response = "https://didcomm.org/didexchange/1.0/response";
        
            /// <summary>
            /// DidExchange Complete Message Type
            /// </summary>
            public const string Complete = "https://didcomm.org/didexchange/1.0/complete";

            /// <summary>
            /// DidExchange Problem Report Message Type
            /// </summary>
            public const string ProblemReport = "https://didcomm.org/didexchange/1.0/problem_report";
        }

        /// <summary>
        /// Issue Credential
        /// </summary>
        public static class IssueCredentialNames
        {
            /// <summary>
            /// Credential Acknowledge Message Type Name
            /// </summary>
            public const string AcknowledgeCredential = "https://didcomm.org/issue-credential/1.0/ack";

            /// <summary>
            /// Credential Propose Message Type Name
            /// </summary>
            public const string ProposeCredential = "https://didcomm.org/issue-credential/1.0/propose-credential";

            /// <summary>
            /// Credential Preview Message Type Name
            /// </summary>
            public const string PreviewCredential = "https://didcomm.org/issue-credential/1.0/credential-preview";

            /// <summary>
            /// Offer Credential Message Type Name
            /// </summary>
            public const string OfferCredential = "https://didcomm.org/issue-credential/1.0/offer-credential";

            /// <summary>
            /// Request Credential Message Type Name
            /// </summary>
            public const string RequestCredential = "https://didcomm.org/issue-credential/1.0/request-credential";

            /// <summary>
            /// Issue Credential Message Type Name
            /// </summary>
            public const string IssueCredential = "https://didcomm.org/issue-credential/1.0/issue-credential";
        }

        /// <summary>
        /// Type names for Present Proof Protocol
        /// </summary>
        public static class PresentProofNames
        {
            /// <summary>
            /// Acknowledge Presentation Message Type Name
            /// </summary>
            public const string AcknowledgePresentation = "https://didcomm.org/present-proof/1.0/ack";
            
            /// <summary>
            /// Propose Presentation Message Type Name
            /// </summary>
            public const string ProposePresentation = "https://didcomm.org/present-proof/1.0/propose-presentation";

            /// <summary>
            /// Request Presentation Message Type Name
            /// </summary>
            public const string RequestPresentation = "https://didcomm.org/present-proof/1.0/request-presentation";

            /// <summary>
            /// Presentation Message Type Name
            /// </summary>
            public const string Presentation = "https://didcomm.org/present-proof/1.0/presentation";

            /// <summary>
            /// Presentation Preview Message Type Name
            /// </summary>
            public const string PresentationPreview = "https://didcomm.org/present-proof/1.0/presentation-preview";
        }
    }
}
