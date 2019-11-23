using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Routing
{
    public class CreateInboxMessage : AgentMessage
    {
        public CreateInboxMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.CreateInboxMessage;
        }

        public Dictionary<string, string> Metadata { get; set; }
    }
}