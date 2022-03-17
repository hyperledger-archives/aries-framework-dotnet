using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Features.IssueCredential.Models.Messages;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Credential Service
    /// </summary>
    public interface ICredentialService
    {
        /// <summary>
        /// Gets credential record for the given identifier.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialId">The credential identifier.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The stored credential record</returns>
        Task<CredentialRecord> GetAsync(IAgentContext agentContext, string credentialId);

        /// <summary>
        /// Retrieves a list of <see cref="CredentialRecord"/> items for the given search criteria.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="query">The query.</param>
        /// <param name="count">The number of items to return</param>
        /// <param name="skip">The number of items to skip</param>
        /// <returns>A list of credential records matching the search criteria</returns>
        Task<List<CredentialRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100, int skip = 0);

        /// <summary>
        /// Process the offer and stores in the designated wallet asynchronous.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialOffer">The credential offer.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>The credential identifier of the stored credential record.</returns>
        Task<string> ProcessOfferAsync(IAgentContext agentContext, CredentialOfferMessage credentialOffer,
            ConnectionRecord connection);

        /// <summary>
        /// Create a credential request for the given record on a previously received offer.
        /// The credential record must be in state "Offered".
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialId">The offer identifier.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.A2AMessageTransmissionError.</exception>
        /// <returns>The response async.</returns>
        Task<(CredentialRequestMessage, CredentialRecord)> CreateRequestAsync(IAgentContext agentContext, string credentialId);

        /// <summary>
        /// Create a credential based on an offer message. This is method is used for connectionless credential exchange.
        /// The credential request message will be sent over transport with return routing and an issued credential is
        /// expected in the response.
        /// If successful, this method will return a credential record in "Issued" state.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="message"></param>
        /// <returns>A credential record that contains the final issued credential.</returns>
        Task<CredentialRecord> CreateCredentialAsync(IAgentContext agentContext, CredentialOfferMessage message);

        /// <summary>
        /// Rejects a credential offer asynchronous.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="offerId">The offer identifier.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The response async.</returns>
        Task RejectOfferAsync(IAgentContext agentContext, string offerId);

        /// <summary>
        /// Processes the issued credential and stores in the designated wallet.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credential">The credential.</param>
        /// <param name="connection">The connection.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The identifier for the credential record.</returns>
        Task<string> ProcessCredentialAsync(IAgentContext agentContext, CredentialIssueMessage credential,
            ConnectionRecord connection);

        /// <summary>
        /// Create a new credential offer for the specified connection. If "connectionId" is 
        /// <c>null</c> this offer must be delivered over connectionless transport.
        /// The credential data will be stored in a tag named "CredentialOfferData" that can be retrieved
        /// at a later stage.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="config">A configuration object used to configure the resulting offers presentation.</param>
        /// <param name="connectionId">The connection id.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The offer message and the identifier. </returns>
        Task<(CredentialOfferMessage, CredentialRecord)> CreateOfferAsync(
            IAgentContext agentContext, OfferConfiguration config, string connectionId);

        /// <summary>
        /// Create a new credential offer for connectionless transport.
        /// The credential data will be stored in a tag named "CredentialOfferData" that can be retrieved
        /// at a later stage. The output <see cref="CredentialOfferMessage" /> will have 
        /// the "~service" decorator set.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="config">A configuration object used to configure the resulting offers presentation.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The offer message and the identifier. </returns>
        Task<(CredentialOfferMessage, CredentialRecord)> CreateOfferAsync(
            IAgentContext agentContext, OfferConfiguration config);

        /// <summary>
        /// Revokes a credential offer.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="offerId">Id of the credential offer.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordInInvalidState.</exception>
        /// <returns>The response async.</returns>
        Task RevokeCredentialOfferAsync(IAgentContext agentContext, string offerId);

        /// <summary>
        /// Processes the credential request and stores in the designated wallet.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialRequest">The credential request.</param>
        /// <param name="connection">The connection.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The credential identifier of the stored credential record.</returns>
        Task<string> ProcessCredentialRequestAsync(IAgentContext agentContext,
            CredentialRequestMessage credentialRequest, ConnectionRecord connection);

        /// <summary>
        /// Creates a credential with the given credential identifier
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialId">The credential identifier.</param>
        /// <returns>The response async.</returns>
        Task<(CredentialIssueMessage, CredentialRecord)> CreateCredentialAsync(IAgentContext agentContext, string credentialId);

        /// <summary>
        /// Creates a credential with the given credential identifier. 
        /// The credential is issued with the attributeValues provided.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialId">The credential request identifier.</param>
        /// <param name="values">Values.</param>
        /// <returns>The response async.</returns>
        Task<(CredentialIssueMessage, CredentialRecord)> CreateCredentialAsync(IAgentContext agentContext, string credentialId,
            IEnumerable<CredentialPreviewAttribute> values);

        /// <summary>
        /// Rejects a credential request asynchronous.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialId">The credential identifier.</param>
        /// <exception cref="AriesFrameworkException">Throws with ErrorCode.RecordNotFound.</exception>
        /// <returns>The response async.</returns>
        Task RejectCredentialRequestAsync(IAgentContext agentContext, string credentialId);

        /// <summary>
        /// Revokes an issued credentials and writes the updated revocation state to the ledger
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialId">Identifier of the credential to be revoked.</param>
        /// <param name="sendRevocationNotification">If true sends a Revocation Notification to the holder</param>
        /// <returns>The response async.</returns>
        Task RevokeCredentialAsync(IAgentContext agentContext, string credentialId, bool sendRevocationNotification = false);

        /// <summary>
        /// Deletes the credential and it's associated record
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="credentialId"></param>
        /// <returns></returns>
        Task DeleteCredentialAsync(IAgentContext agentContext, string credentialId);
        
        /// <summary>
        /// Creates a Credential Acknowledgement Message async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialRecordId">The ID of the credential record.</param>
        /// <param name="status">The status of the acknowledgement message</param>
        /// <returns>The acknowledgement message</returns>
        Task<CredentialAcknowledgeMessage> CreateAcknowledgementMessageAsync(IAgentContext agentContext, string credentialRecordId, string status = AcknowledgementStatusConstants.Ok);

        /// <summary>
        /// Processes a Credential Acknowledgement Message async. 
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="credentialAcknowledgeMessage">The credential acknowledgement message.</param>
        /// <returns>The record associated with the acknowledgement message</returns>
        Task<CredentialRecord> ProcessAcknowledgementMessageAsync(IAgentContext agentContext, CredentialAcknowledgeMessage credentialAcknowledgeMessage);
    }
}
