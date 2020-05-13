using System;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    /// <summary>
    /// Add Route Message
    /// </summary>
    /// <seealso cref="Hyperledger.Aries.Agents.AgentMessage" />
    public class AddRouteMessage : AgentMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRouteMessage"/> class.
        /// </summary>
        public AddRouteMessage()
        {
            Id = Guid.NewGuid().ToString();
            Type = RoutingTypeNames.AddRouteMessage;
        }
        /// <summary>
        /// Gets or sets the route destination.
        /// </summary>
        /// <value>
        /// The route destination.
        /// </value>
        public string RouteDestination { get; set; }
    }
}