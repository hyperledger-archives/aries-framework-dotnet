using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Common;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof.Messages;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries.Features.PresentProof
{
    /// <summary>
    /// Proof Service.
    /// </summary>
    public interface IProofService
    {
        /// <summary>
        /// Creates a proof proposal.
        /// </summary>
        /// <returns>The proof proposal.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofProposal">The proof proposal that will be created</param>
        /// <param name="connectionId">Connection identifier of who the proof request will be sent to.</param>
        /// <returns>Proof Request message and identifier.</returns>
        Task<(ProposePresentationMessage, ProofRecord)> CreateProposalAsync(IAgentContext agentContext,
            ProofProposal proofProposal, string connectionId);


        /// <summary>
        /// Creates a proof request from a proof proposal.
        /// </summary>
        /// <returns>The proof request.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="requestParameters">The parameters requested to be proven</param>
        /// <param name="proofRecordId">proposal Id</param>
        /// <param name="connectionId">Connection identifier of who the proof proposal will be sent to.</param>
        /// <returns>Proof Request message and identifier.</returns>
        Task<(RequestPresentationMessage, ProofRecord)> CreateRequestFromProposalAsync(IAgentContext agentContext, ProofRequestParameters requestParameters,
            string proofRecordId, string connectionId);


        /// <summary>
        /// Processes a proof proposal and stores it for a given connection.
        /// </summary>
        /// <returns>The identifier for the stored proof proposal.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proposePresentationMessage">A proof proposal.</param>
        /// <param name="connection">Connection.</param>
        /// <returns>Proof identifier.</returns>
        Task<ProofRecord> ProcessProposalAsync(IAgentContext agentContext, ProposePresentationMessage proposePresentationMessage, ConnectionRecord connection);


        /// <summary>
        /// Creates a proof request.
        /// </summary>
        /// <returns>The proof request.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRequest">An enumeration of attribute we wish the prover to disclose.</param>
        /// <param name="connectionId">Connection identifier of who the proof request will be sent to.</param>
        /// <returns>Proof Request message and identifier.</returns>
        Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext,
            ProofRequest proofRequest, string connectionId);

        /// <summary>
        /// Creates a new <see cref="RequestPresentationMessage" /> and associated <see cref="ProofRecord" />
        /// for use with connectionless transport.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="proofRequest"></param>
        /// <param name="useDidKeyFormat"></param>
        /// <returns></returns>
        Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext, ProofRequest proofRequest, bool useDidKeyFormat = false);

        /// <summary>
        /// Creates a proof request.
        /// </summary>
        /// <returns>The proof request.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRequestJson">A string representation of proof request json object</param>
        /// <param name="connectionId">Connection identifier of who the proof request will be sent to.</param>
        /// <returns>Proof Request message and identifier.</returns>
        Task<(RequestPresentationMessage, ProofRecord)> CreateRequestAsync(IAgentContext agentContext,
            string proofRequestJson, string connectionId);

        /// <summary>
        /// Processes a proof request and stores it for a given connection.
        /// </summary>
        /// <returns>The identifier for the stored proof request.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRequest">A proof request.</param>
        /// <param name="connection">Connection.</param>
        /// <returns>Proof identifier.</returns>
        Task<ProofRecord> ProcessRequestAsync(IAgentContext agentContext, RequestPresentationMessage proofRequest, ConnectionRecord connection);

        /// <summary>
        /// Processes a proof and stores it for a given connection.
        /// </summary>
        /// <returns>The identifier for the stored proof.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proof">A proof.</param>
        /// <returns>Proof identifier.</returns>
        Task<ProofRecord> ProcessPresentationAsync(IAgentContext agentContext, PresentationMessage proof);

        /// <summary>
        /// Processes a proof request and generates an accompanying proof.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRequest">Proof Request.</param>
        /// <param name="requestedCredentials">Requested Credentials.</param>
        /// <returns>
        /// The proof.
        /// </returns>
        Task<string> CreatePresentationAsync(
            IAgentContext agentContext,
            ProofRequest proofRequest,
            RequestedCredentials requestedCredentials);

        /// <summary>
        /// Creates a presentation message based on a request over connectionless transport.
        /// </summary>
        /// <param name="agentContext"></param>
        /// <param name="requestPresentation"></param>
        /// <param name="requestedCredentials"></param>
        /// <returns></returns>
        Task<ProofRecord> CreatePresentationAsync(IAgentContext agentContext, RequestPresentationMessage requestPresentation, RequestedCredentials requestedCredentials);

        /// <summary>
        /// Creates a proof.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRecordId">Identifier of the proof request.</param>
        /// <param name="requestedCredentials">The requested credentials.</param>
        /// <returns>
        /// The proof.
        /// </returns>
        Task<(PresentationMessage, ProofRecord)> CreatePresentationAsync(IAgentContext agentContext,
            string proofRecordId, RequestedCredentials requestedCredentials);

        /// <summary>
        /// Rejects a proof request.
        /// </summary>
        /// <returns>The proof.</returns>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRecordId">Identifier of the proof request.</param>
        Task RejectProofRequestAsync(IAgentContext agentContext, string proofRecordId);

        /// <summary>
        /// Verifies a proof.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRecordId">Identifier of the proof record.</param>
        /// <returns>Status indicating validity of proof.</returns>
        Task<bool> VerifyProofAsync(IAgentContext agentContext, string proofRecordId);

        /// <summary>
        /// Check if a credential has been revoked by validating against the revocation state
        /// on the ledger.
        /// </summary>
        /// <remarks>
        /// This method can be used as a holder, to check if a credential thay own has been
        /// revoked by the issuer. If a credential doesn't support revocation, this method will
        /// always return <c>false</c>
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        Task<bool> IsRevokedAsync(IAgentContext context, CredentialRecord record);

        /// <summary>
        /// Check if a credential has been revoked by validating against the revocation state
        /// on the ledger.
        /// </summary>
        /// <remarks>
        /// This method can be used as a holder, to check if a credential thay own has been
        /// revoked by the issuer. If a credential doesn't support revocation, this method will
        /// always return <c>false</c>
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="credentialRecordId"></param>
        /// <returns></returns>
        Task<bool> IsRevokedAsync(IAgentContext context, string credentialRecordId);

        /// <summary>
        /// Verifies a proof.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRequestJson">The proof request.</param>
        /// <param name="proofJson">The proof.</param>
        /// <param name="validateEncoding">
        /// If <c>true</c>, validate the encoded raw values against standard encoding.
        /// It is recommended to enable encoding validation to detect raw value tampering.
        /// </param>
        /// <returns>Status indiciating the validity of proof.</returns>
        Task<bool> VerifyProofAsync(IAgentContext agentContext, string proofRequestJson, string proofJson, bool validateEncoding = true);

        /// <summary>
        /// Gets an enumeration of proofs stored in the wallet.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="query">The query.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// A list of proofs.
        /// </returns>
        Task<List<ProofRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100);

        /// <summary>
        /// Gets a particular proof stored in the wallet.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRecId">Identifier of the proof record.</param>
        /// <returns>The proof.</returns>
        Task<ProofRecord> GetAsync(IAgentContext agentContext, string proofRecId);

        /// <summary>
        /// Lists the credentials available for the given proof request.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRequest">The proof request object.</param>
        /// <param name="attributeReferent">The attribute referent.</param>
        /// <returns>
        /// A collection of <see cref="CredentialInfo" /> that are available
        /// for building a proof for the given proof request
        /// </returns>
        Task<List<Credential>> ListCredentialsForProofRequestAsync(
            IAgentContext agentContext,
            ProofRequest proofRequest,
            string attributeReferent);

        /// <summary>
        /// Creates a presentation acknowledge message async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="proofRecordId">The ID of the proof record that is used.</param>
        /// <param name="status">The status of the acknowledgement message</param>
        /// <returns>The Presentation Acknowledgement Message.</returns>
        Task<PresentationAcknowledgeMessage> CreateAcknowledgeMessageAsync(IAgentContext agentContext, string proofRecordId, string status = AcknowledgementStatusConstants.Ok);

        /// <summary>
        /// Processes a presentation acknowledge message async.
        /// </summary>
        /// <param name="agentContext">Agent Context.</param>
        /// <param name="presentationAcknowledgeMessage">The presentation acknowledgement message.</param>
        /// <returns>The proof record associated with the acknowledge message.</returns>
        Task<ProofRecord> ProcessAcknowledgeMessageAsync(IAgentContext agentContext, PresentationAcknowledgeMessage presentationAcknowledgeMessage);
    }
}
