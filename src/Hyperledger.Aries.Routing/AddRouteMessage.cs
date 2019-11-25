using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public class AddRouteMessage : AgentMessage
    {
        public AddRouteMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.AddRouteMessage;
        }
        public string RouteDestination { get; set; }
    }
}