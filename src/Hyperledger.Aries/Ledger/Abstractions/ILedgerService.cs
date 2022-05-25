using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Ledger.Models;
using Hyperledger.Aries.Payments;
using Hyperledger.Indy.LedgerApi;

namespace Hyperledger.Aries.Contracts
{
    /// <summary>
    /// Ledger service.
    /// </summary>
    public interface ILedgerService
    {
        /// <summary>
        /// Gets a list of all authorization rules for the given pool
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <returns></returns>
        Task<IList<AuthorizationRule>> LookupAuthorizationRulesAsync(IAgentContext agentContext);

        /// <summary>
        /// Looks up an attribute value on the ledger.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="targetDid">The target DID for the <paramref name="attributeName" /> lookup</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <returns>
        /// The attribute value or <c>null</c> if none were found.
        /// </returns>
        Task<string> LookupAttributeAsync(IAgentContext agentContext, string targetDid, string attributeName);

        /// <summary>
        /// Register an attribute for the specified <paramref name="targetDid"/> to the ledger.
        /// </summary>
        /// <returns>The attribute async.</returns>
        /// <param name="context">Agent context.</param>
        /// <param name="submittedDid">Submitted did.</param>
        /// <param name="targetDid">Target did.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="paymentInfo">Payment information</param>
        Task RegisterAttributeAsync(IAgentContext context, string submittedDid, string targetDid,
            string attributeName, object value, TransactionCost paymentInfo = null);

        /// <summary>
        /// Lookup the schema async.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="schemaId">Schema identifier.</param>
        /// <returns>
        /// The schema async.
        /// </returns>
        Task<ParseResponseResult> LookupSchemaAsync(IAgentContext agentContext, string schemaId);

        /// <summary>
        /// Lookup NYM record on the ledger
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="did">The did.</param>
        /// <returns></returns>
        Task<string> LookupNymAsync(IAgentContext agentContext, string did);

        /// <summary>
        /// Lookup the ledger transaction async.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="ledgerType">The ledger type.</param>
        /// <param name="sequenceId">The sequence identifier.</param>
        /// <returns>
        /// The transaction async.
        /// </returns>
        Task<string> LookupTransactionAsync(IAgentContext agentContext, string ledgerType, int sequenceId);

        /// <summary>
        /// Lookups the definition async.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="definitionId">Definition identifier.</param>
        /// <returns>
        /// The definition async.
        /// </returns>
        Task<ParseResponseResult> LookupDefinitionAsync(IAgentContext agentContext, string definitionId);

        /// <summary>
        /// Lookups the revocation registry definition.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="registryId">The registry identifier.</param>
        /// <returns></returns>
        Task<ParseResponseResult> LookupRevocationRegistryDefinitionAsync(IAgentContext agentContext, string registryId);

        /// <summary>
        /// Lookup the revocation registry delta for the given registry in the range specified.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="revocationRegistryId">Revocation registry identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>
        /// The revocation registry delta.
        /// </returns>
        Task<ParseRegistryResponseResult> LookupRevocationRegistryDeltaAsync(IAgentContext agentContext, string revocationRegistryId,
            long from, long to);

        /// <summary>
        /// Lookup revocation registry for the given point in time.
        /// </summary>
        /// <param name="agentContext">The agent context.</param>
        /// <param name="revocationRegistryId">Revocation registry identifier.</param>
        /// <param name="timestamp">Timestamp.</param>
        /// <returns>
        /// The revocation registry async.
        /// </returns>
        Task<ParseRegistryResponseResult> LookupRevocationRegistryAsync(IAgentContext agentContext, string revocationRegistryId,
            long timestamp);

        /// <summary>
        /// Registers the nym async.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="submitterDid">The submitter did.</param>
        /// <param name="theirDid">Their did.</param>
        /// <param name="theirVerkey">Their verkey.</param>
        /// <param name="role">Role the new nym will assume.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns>
        /// Registration async.
        /// </returns>
        Task RegisterNymAsync(IAgentContext context, string submitterDid, string theirDid,
            string theirVerkey, string role, TransactionCost paymentInfo = null);

        /// <summary>
        /// Registers the credential definition async.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="submitterDid">The submitter did.</param>
        /// <param name="data">Data.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns>
        /// The credential definition async.
        /// </returns>
        Task RegisterCredentialDefinitionAsync(IAgentContext context, string submitterDid,
            string data, TransactionCost paymentInfo = null);

        /// <summary>
        /// Registers the revocation registry definition asynchronous.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="submitterDid">The submitter did.</param>
        /// <param name="data">The data.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns></returns>
        Task RegisterRevocationRegistryDefinitionAsync(IAgentContext context, string submitterDid,
            string data, TransactionCost paymentInfo = null);

        /// <summary>
        /// Sends the revocation registry entry asynchronous.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="issuerDid">The issuer did.</param>
        /// <param name="revocationRegistryDefinitionId">The revocation registry definition identifier.</param>
        /// <param name="revocationDefinitionType">Type of the revocation definition.</param>
        /// <param name="value">The value.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns></returns>
        Task SendRevocationRegistryEntryAsync(IAgentContext context, string issuerDid,
            string revocationRegistryDefinitionId, string revocationDefinitionType, string value,
            TransactionCost paymentInfo = null);

        /// <summary>
        /// Registers the schema asynchronous.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="issuerDid">The issuer did.</param>
        /// <param name="schemaJson">The schema json.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns></returns>
        Task RegisterSchemaAsync(IAgentContext context, string issuerDid, string schemaJson,
            TransactionCost paymentInfo = null);

        /// <summary>
        /// Lookup service endpoint information for a did on a public ledger
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="did">The did.</param>
        /// <returns></returns>
        Task<ServiceEndpointResult> LookupServiceEndpointAsync(IAgentContext context, string did);

        /// <summary>
        /// Register service endpoint information for a did on a public ledger
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="did">The destination did.</param>
        /// <param name="serviceEndpoint">The endpoint information to be added</param>
        /// <param name="paymentInfo">Payment information.</param>
        /// <returns></returns>
        Task RegisterServiceEndpointAsync(IAgentContext context, string did, string serviceEndpoint,
            TransactionCost paymentInfo = null);
    }
}
