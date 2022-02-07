using System;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;

namespace Hyperledger.Aries.Features.RevocationNotification
{
    /// <summary>
    /// Acknowledgement message for revocation notifications
    /// </summary>
    public class RevocationNotificationAcknowledgeMessage : AcknowledgeMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RevocationNotificationAcknowledgeMessage"/> class.
        /// </summary>
        public RevocationNotificationAcknowledgeMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps
                ? MessageTypesHttps.RevocationNotificationAcknowledgement
                : MessageTypes.RevocationNotificationAcknowledgement;
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="RevocationNotificationAcknowledgeMessage"/> class.
        /// </summary>
        public RevocationNotificationAcknowledgeMessage(bool useMessagesTypesHttps = false) : base(useMessagesTypesHttps)
        {
            Id = Guid.NewGuid().ToString();
            Type = UseMessageTypesHttps
                ? MessageTypesHttps.RevocationNotificationAcknowledgement
                : MessageTypes.RevocationNotificationAcknowledgement;
        }
    }
}
