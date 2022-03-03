using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.DidExchange.Models;

namespace Hyperledger.Aries.Features.Handshakes.DidExchange
{
    /// <summary>
    /// Did Exchange service.
    /// </summary>
    public interface IDidExchangeService
    {
        /// <summary>
        /// Not implemented yet
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="message">The invitation message.</param>
        /// <returns>The did exchange request message</returns>
        Task<(DidExchangeRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, AgentMessage message);
        
        /// <summary>
        /// Create did exchange request based on a public resolvable did.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="did">A public resolvable did.</param>
        /// <returns>The did exchange request message</returns>
        Task<(DidExchangeRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, string did);

        /// <summary>
        /// Process a did exchange request message.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="requestMessage">The did exchange request message.</param>
        /// <returns>The connection record.</returns>
        Task<ConnectionRecord> ProcessRequestAsync(IAgentContext agentContext, DidExchangeRequestMessage requestMessage);

        /// <summary>
        /// Create did exchange response message. 
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="connectionRecord">The connection record.</param>
        /// <returns>The did exchange response message.</returns>
        Task<(DidExchangeResponseMessage, ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, ConnectionRecord connectionRecord);
        
        /// <summary>
        /// Process did exchange response message. 
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="connectionRecord">The connection record.</param>
        /// <returns>The connection record.</returns>
        Task<ConnectionRecord> ProcessResponseAsync(IAgentContext agentContext, DidExchangeResponseMessage responseMessage, ConnectionRecord connectionRecord);

        /// <summary>
        /// Create did exchange complete message.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="connectionRecord">The connection record.</param>
        /// <returns>The did exchange complete message.</returns>
        Task<(DidExchangeCompleteMessage, ConnectionRecord)> CreateComplete(IAgentContext agentContext, ConnectionRecord connectionRecord);

        /// <summary>
        /// Process did exchange complete message.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="completeMessage">The did exchange complete message.</param>
        /// <param name="connectionRecord">The connection record.</param>
        /// <returns>The connection record.</returns>
        Task<ConnectionRecord> ProcessComplete(IAgentContext agentContext, DidExchangeCompleteMessage completeMessage, ConnectionRecord connectionRecord);
        
        /// <summary>
        /// Abandon the did exchange protocol.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="connectionRecord">The connection record.</param>
        /// <returns>The problem report message.</returns>
        Task<(DidExchangeProblemReportMessage, ConnectionRecord)> AbandonDidExchange(IAgentContext agentContext, ConnectionRecord connectionRecord);

        /// <summary>
        /// Process problem report message.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="problemReportMessage">The problem report message.</param>
        /// <param name="connectionRecord">The connection record.</param>
        /// <returns>The connection record.</returns>
        Task<ConnectionRecord> ProcessProblemReportMessage(IAgentContext agentContext, DidExchangeProblemReportMessage problemReportMessage, ConnectionRecord connectionRecord);
    }
}
