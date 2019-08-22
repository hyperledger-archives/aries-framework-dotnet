using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages.Connections;
using AgentFramework.Core.Models.Connections;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Connection Service.
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// Gets the async.
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionId">Connection identifier.</param>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The connection record.</returns>
        Task<ConnectionRecord> GetAsync(IAgentContext agentContext, string connectionId);

        /// <summary>
        /// Retrieves a list of <see cref="ConnectionRecord"/> items for the given search criteria
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="query">The query used to filter the search results.</param>
        /// <param name="count">The maximum item count of items to return to return.</param>
        Task<List<ConnectionRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100);

        /// <summary>
        /// Creates the invitation asynchronous.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="config">An optional configuration object used to configure the resulting invitations presentation</param>
        /// <returns>The async.</returns>
        Task<(ConnectionInvitationMessage, ConnectionRecord)> CreateInvitationAsync(IAgentContext agentContext, InviteConfiguration config = null);

        /// <summary>
        /// Revokes an invitation.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="invitationId">Id of the invitation.</param>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The async.</returns>
        Task RevokeInvitationAsync(IAgentContext agentContext, string invitationId);

        /// <summary>
        /// Accepts the connection invitation async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="offer">Offer.</param>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError.</exception>
        /// <returns>Connection identifier unique for this connection.</returns>
        Task<(ConnectionRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionInvitationMessage offer);

        /// <summary>
        /// Process the connection request for a given connection async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="request">Request.</param>
        /// <param name="connection">Connection.</param>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError.</exception>
        /// <returns>Connection identifier this request is related to.</returns>
        Task<string> ProcessRequestAsync(IAgentContext agentContext, ConnectionRequestMessage request, ConnectionRecord connection);

        /// <summary>
        /// Accepts the connection request and sends a connection response
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionId">Connection identifier.</param>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError</exception>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AgentFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The response async.</returns>
        Task<(ConnectionResponseMessage,ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, string connectionId);

        /// <summary>
        /// Processes the connection response for a given connection async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="response">Response.</param>
        /// <param name="connection">Connection.</param>
        /// <returns>Connection identifier this request is related to.</returns>
        Task<string> ProcessResponseAsync(IAgentContext agentContext, ConnectionResponseMessage response, ConnectionRecord connection);

        /// <summary>
        /// Deletes a connection from the local store
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionId">Connection Identifier.</param>
        /// <returns>The response async with a boolean indicating if deletion occured successfully</returns>
        Task<bool> DeleteAsync(IAgentContext agentContext, string connectionId);
    }
}
