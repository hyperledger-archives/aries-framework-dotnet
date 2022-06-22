using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.OutOfBand
{
    public class HandshakeReuseMessage : AgentMessage
    {
        public HandshakeReuseMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.OutOfBand.HandshakeReuse;
            UseMessageTypesHttps = true;
        }
    }
}
