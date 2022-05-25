using Hyperledger.Aries.Features.Handshakes.Connection.Models;

namespace Hyperledger.Aries.Routing
{
    /// <summary>
    /// Agent Public Configuration
    /// </summary>
    public class AgentPublicConfiguration
    {
        /// <summary>
        /// Gets or sets the service endpoint.
        /// </summary>
        /// <value>
        /// The service endpoint.
        /// </value>
        public string ServiceEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the routing key.
        /// </summary>
        /// <value>
        /// The routing key.
        /// </value>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Gets or sets the invitation.
        /// </summary>
        /// <value>
        /// The invitation.
        /// </value>
        public ConnectionInvitationMessage Invitation { get; set; }
    }
}
