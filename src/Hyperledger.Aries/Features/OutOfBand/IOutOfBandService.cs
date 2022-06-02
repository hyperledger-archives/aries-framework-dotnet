using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;

namespace Hyperledger.Aries.Features.OutOfBand
{
    /// <summary>
    /// Out-of-band service 
    /// </summary>
    public interface IOutOfBandService
    {
        /// <summary>
        /// Create an out-of-band invitation and an associated ConnectionRecord
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="requests">An enumerable of requests to attach.</param>
        /// <param name="config">A configuration for the invite.</param>
        /// <returns></returns>
        Task<(InvitationMessage, ConnectionRecord)> CreateInvitationAsync(IAgentContext agentContext, IEnumerable<AgentMessage> requests = null, InviteConfiguration config = null);

        /// <summary>
        /// Process an out-of-band invitation message
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="invitation">The invitation.</param>
        /// <returns>A <see cref="HandshakeReuseAcceptedMessage"/> if there is an existing connection for the given DID
        /// and the according <see cref="ConnectionRecord"/>. Otherwise it will only return a new <see cref="ConnectionRecord"/>.</returns>
        Task<(HandshakeReuseMessage, ConnectionRecord)> ProcessInvitationMessage(IAgentContext agentContext, InvitationMessage invitation);

        /// <summary>
        /// Process a HandshakeReuseMessage 
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="handshakeReuseMessage">The message.</param>
        /// <returns><see cref="HandshakeReuseAcceptedMessage"/></returns>
        Task<HandshakeReuseAcceptedMessage> ProcessHandshakeReuseMessage(IAgentContext agentContext, HandshakeReuseMessage handshakeReuseMessage);

        /// <summary>
        /// Process a HandshakeReuseAcceptedMessage
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="handshakeReuseAcceptedMessage">The agent context.</param>
        /// <returns></returns>
        Task ProcessHandshakeReuseAccepted(IAgentContext agentContext, HandshakeReuseAcceptedMessage handshakeReuseAcceptedMessage);
    }
}
