using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.OutOfBand
{
    public class HandshakeReuseAcceptedMessage : AgentMessage
    {
        public HandshakeReuseAcceptedMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.OutOfBand.HandshakeReuse;
            UseMessageTypesHttps = true;
        }
    }
}
