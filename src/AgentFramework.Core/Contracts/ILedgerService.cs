using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Payments;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Ledger service.
    /// </summary>
    public interface ILedgerService
    {
        /// <summary>
        /// Gets a list of all authorization rules for the given pool
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        Task<IList<AuthorizationRule>> LookupAuthorizationRulesAsync(Pool pool);

        /// <summary>
        /// Looks up an attribute value on the ledger.
        /// </summary>
        /// <returns>The attribute value or <c>null</c> if none were found.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="targetDid">The target DID for the <paramref name="attributeName"/> lookup</param>
        /// <param name="attributeName">Attribute name.</param>
        Task<string> LookupAttributeAsync(Pool pool, string targetDid, string attributeName);

        /// <summary>
        /// Register an attribute for the specified <paramref name="targetDid"/> to the ledger.
        /// </summary>
        /// <returns>The attribute async.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="wallet">Wallet.</param>
        /// <param name="submittedDid">Submitted did.</param>
        /// <param name="targetDid">Target did.</param>
        /// <param name="attributeName">Attribute name.</param>
        /// <param name="value">The attribute value.</param>
        /// <param name="paymentInfo">Payment information</param>
        Task RegisterAttributeAsync(Pool pool, Wallet wallet, string submittedDid, string targetDid,
            string attributeName, object value, TransactionCost paymentInfo = null);

        /// <summary>
        /// Lookup the schema async.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="schemaId">Schema identifier.</param>
        /// <returns>
        /// The schema async.
        /// </returns>
        Task<ParseResponseResult> LookupSchemaAsync(Pool pool, string schemaId);

        /// <summary>
        /// Lookup NYM record on the ledger
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="did"></param>
        /// <returns></returns>
        Task<string> LookupNymAsync(Pool pool, string did);

        /// <summary>
        /// Lookup the ledger transaction async.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="ledgerType">The ledger type.</param>
        /// <param name="sequenceId">The sequence identifier.</param>
        /// <returns>
        /// The transaction async.
        /// </returns>
        Task<string> LookupTransactionAsync(Pool pool, string ledgerType, int sequenceId);

        /// <summary>
        /// Lookups the definition async.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="definitionId">Definition identifier.</param>
        /// <returns>
        /// The definition async.
        /// </returns>
        Task<ParseResponseResult> LookupDefinitionAsync(Pool pool, string definitionId);

        /// <summary>
        /// Lookups the revocation registry definition.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="registryId">The registry identifier.</param>
        /// <returns></returns>
        Task<ParseResponseResult> LookupRevocationRegistryDefinitionAsync(Pool pool, string registryId);

        /// <summary>
        /// Lookup the revocation registry delta for the given registry in the range specified.
        /// </summary>
        /// <returns>The revocation registry delta.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="revocationRegistryId">Revocation registry identifier.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        Task<ParseRegistryResponseResult> LookupRevocationRegistryDeltaAsync(Pool pool, string revocationRegistryId,
            long from, long to);

        /// <summary>
        /// Lookup revocation registry for the given point in time.
        /// </summary>
        /// <returns>The revocation registry async.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="revocationRegistryId">Revocation registry identifier.</param>
        /// <param name="timestamp">Timestamp.</param>
        Task<ParseRegistryResponseResult> LookupRevocationRegistryAsync(Pool pool, string revocationRegistryId,
            long timestamp);

        /// <summary>
        /// Registers the nym async.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="pool">The pool.</param>
        /// <param name="submitterDid">The submitter did.</param>
        /// <param name="theirDid">Their did.</param>
        /// <param name="theirVerkey">Their verkey.</param>
        /// <param name="role">Role the new nym will assume.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns>
        /// Registration async.
        /// </returns>
        Task RegisterNymAsync(Wallet wallet, Pool pool, string submitterDid, string theirDid,
            string theirVerkey, string role, TransactionCost paymentInfo = null);

        /// <summary>
        /// Registers the credential definition async.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="pool">The pool.</param>
        /// <param name="submitterDid">The submitter did.</param>
        /// <param name="data">Data.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns>
        /// The credential definition async.
        /// </returns>
        Task RegisterCredentialDefinitionAsync(Wallet wallet, Pool pool, string submitterDid,
            string data, TransactionCost paymentInfo = null);

        /// <summary>
        /// Registers the revocation registry definition asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="pool">The pool.</param>
        /// <param name="submitterDid">The submitter did.</param>
        /// <param name="data">The data.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns></returns>
        Task RegisterRevocationRegistryDefinitionAsync(Wallet wallet, Pool pool, string submitterDid,
            string data, TransactionCost paymentInfo = null);

        /// <summary>
        /// Sends the revocation registry entry asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="pool">The pool.</param>
        /// <param name="issuerDid">The issuer did.</param>
        /// <param name="revocationRegistryDefinitionId">The revocation registry definition identifier.</param>
        /// <param name="revocationDefinitionType">Type of the revocation definition.</param>
        /// <param name="value">The value.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns></returns>
        Task SendRevocationRegistryEntryAsync(Wallet wallet, Pool pool, string issuerDid,
            string revocationRegistryDefinitionId, string revocationDefinitionType, string value,
            TransactionCost paymentInfo = null);

        /// <summary>
        /// Registers the schema asynchronous.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="wallet">The wallet.</param>
        /// <param name="issuerDid">The issuer did.</param>
        /// <param name="schemaJson">The schema json.</param>
        /// <param name="paymentInfo">Payment information</param>
        /// <returns></returns>
        Task RegisterSchemaAsync(Pool pool, Wallet wallet, string issuerDid, string schemaJson,
            TransactionCost paymentInfo = null);
    }
}