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
        public RevocationNotificationAcknowledgeMessage() : base(true)
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.RevocationNotificationAcknowledgement;
        }
    }
}
