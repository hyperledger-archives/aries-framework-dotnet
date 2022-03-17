using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange.Models
{
    public class DidExchangeCompleteMessage : AgentMessage
    {
        public DidExchangeCompleteMessage() : base(true)
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.DidExchange.Complete;
        }
    }
}
