﻿namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Protocol message types using "https" scheme
    /// </summary>
    public static class MessageTypesHttps
    {
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
        /// Issue Credential
        /// </summary>
        public static class IssueCredentialNames
        {
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
        public class PresentProofNames
        {
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