using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.Handshakes.Connection
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
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The connection record.</returns>
        Task<ConnectionRecord> GetAsync(IAgentContext agentContext, string connectionId);

        /// <summary>
        /// Retrieves a list of <see cref="ConnectionRecord"/> items for the given search criteria
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="query">The query used to filter the search results.</param>
        /// <param name="count">The maximum item count of items to return to return.</param>
        /// <param name="skip">The number of items to skip.</param>
        Task<List<ConnectionRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100, int skip = 0);

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
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The async.</returns>
        Task RevokeInvitationAsync(IAgentContext agentContext, string invitationId);

        /// <summary>
        /// Process an invitation offer
        /// </summary>
        /// <param name="agentContext">Agent context</param>
        /// <param name="offer">Connection offer message</param>
        /// <returns>The new connection record.</returns>
        Task<ConnectionRecord> ProcessInvitationAsync(IAgentContext agentContext, ConnectionInvitationMessage offer);

        /// <summary>
        /// Process an out-of-band invitation
        /// </summary>
        /// <param name="agentContext">Agent context.</param>
        /// <param name="invitation">Out-of-band invitation message</param>
        /// <returns>The <see cref="ConnectionRecord"/></returns>
        Task<ConnectionRecord> ProcessInvitationAsync(IAgentContext agentContext, InvitationMessage invitation);

        /// <summary>
        /// Accepts an existing invitation.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionRecord">The existing ConnectionRecord.</param>
        /// <returns>The ConnectionRequestMessage and the updated ConnectionRecord.</returns>
        Task<(ConnectionRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionRecord connectionRecord);
        
        /// <summary>
        /// Accepts a connection invitation without existing ConnectionRecord.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="offer">Offer.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError.</exception>
        /// <returns>Connection identifier unique for this connection.</returns>
        Task<(ConnectionRequestMessage, ConnectionRecord)> CreateRequestAsync(IAgentContext agentContext, ConnectionInvitationMessage offer);

        /// <summary>
        /// Process the connection request for a given connection async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="request">Request.</param>
        /// <param name="connection">Connection.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError.</exception>
        /// <returns>Connection identifier this request is related to.</returns>
        Task<string> ProcessRequestAsync(IAgentContext agentContext, ConnectionRequestMessage request, ConnectionRecord connection);

        /// <summary>
        /// Accepts the connection request and sends a connection response
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionId">Connection identifier.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The response async.</returns>
        Task<(ConnectionResponseMessage, ConnectionRecord)> CreateResponseAsync(IAgentContext agentContext, string connectionId);

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

        /// <summary>
        /// Creates a Connection Acknowledgement Message async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionRecordId">The ID of the connection record.</param>
        /// <param name="status">The status of the acknowledgement message</param>
        /// <returns>The connection acknowledge message</returns>
        Task<ConnectionAcknowledgeMessage> CreateAcknowledgementMessageAsync(IAgentContext agentContext, string connectionRecordId, string status = AcknowledgementStatusConstants.Ok);

        /// <summary>
        /// Processes a Connection Acknowledgement Message async. 
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="connectionAcknowledgeMessage">The connection acknowledgement message.</param>
        /// <returns>The record associated with the acknowledgement message</returns>
        Task<ConnectionRecord> ProcessAcknowledgementMessageAsync(IAgentContext agentContext, ConnectionAcknowledgeMessage connectionAcknowledgeMessage);

        /// <summary>
        /// Retrieves a <see cref="ConnectionRecord"/> by key.
        /// </summary>
        /// <returns>The connection record.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="myKey">My verkey.</param>
        /// <exception cref="ArgumentNullException"/>
        Task<ConnectionRecord> ResolveByMyKeyAsync(IAgentContext agentContext, string myKey);
    }
}
