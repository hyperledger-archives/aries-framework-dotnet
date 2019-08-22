using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Records;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Schema Service.
    /// </summary>
    public interface ISchemaService
    {
        /// <summary>
        /// Creates and registers schema on the ledger
        /// </summary>
        /// <param name="context">The agent context</param>
        /// <param name="issuerDid">The issuer did.
        /// <remarks>The DID must have `TRUST_ANCHOR` permissions on the ledger.</remarks></param>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <param name="attributeNames">The attribute names.</param>
        /// <returns>The schema identifier of the stored schema object.
        /// This identifier can be used for ledger schema lookup.</returns>
        Task<string> CreateSchemaAsync(IAgentContext context, string issuerDid, string name, string version,
            string[] attributeNames);

        /// <summary>
        /// Creates and registers schema on the ledger
        /// </summary>
        /// <param name="context">The agent context</param>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <param name="attributeNames">The attribute names.</param>
        /// <returns></returns>
        Task<string> CreateSchemaAsync(IAgentContext context, string name, string version, string[] attributeNames);

        /// <summary>Creates the credential definition and registers it on the ledger.</summary>
        /// <param name="context">The agent context</param>
        /// <param name="schemaId">The schema identifier.</param>
        /// <param name="issuerDid">The issuer did.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="supportsRevocation">if set to <c>true</c> [supports revocation].</param>
        /// <param name="maxCredentialCount">Maximum number of credentials supported by this definition.
        /// This parameter is only used if <paramref name="supportsRevocation" /> is <c>true</c>.</param>
        /// <param name="tailsBaseUri">The public URI of the tails file for the revocation definition.
        /// his parameter is only used if <paramref name="supportsRevocation" /> is <c>true</c>.</param>
        /// <returns>The credential definition identifier of the stored definition record.
        /// This identifier can be used for ledger definition lookup.</returns>
        Task<string> CreateCredentialDefinitionAsync(IAgentContext context, string schemaId, string issuerDid,
            string tag, bool supportsRevocation, int maxCredentialCount, Uri tailsBaseUri);

        /// <summary>Creates the credential definition and registers it on the ledger.</summary>
        /// <param name="context">The agent context</param>
        /// <param name="schemaId">The schema identifier.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="supportsRevocation">if set to <c>true</c> [supports revocation].</param>
        /// <param name="maxCredentialCount">The maximum credential count.</param>
        /// <returns></returns>
        Task<string> CreateCredentialDefinitionAsync(IAgentContext context, string schemaId,
            string tag, bool supportsRevocation, int maxCredentialCount);

        /// <summary>
        /// Gets the schemas asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <returns>A list of schema records that this issuer has created</returns>
        Task<List<SchemaRecord>> ListSchemasAsync(Wallet wallet);

        /// <summary>
        /// Gets the credential definitions asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <returns>A list of credential definition records that this issuer has created</returns>
        Task<List<DefinitionRecord>> ListCredentialDefinitionsAsync(Wallet wallet);

        /// <summary>
        /// Gets the credential definition asynchronous.
        /// </summary>
        /// <param name="wallet">The wallet.</param>
        /// <param name="credentialDefinitionId">The credential definition identifier.</param>
        /// <returns>The credential definition record.</returns>
        Task<DefinitionRecord> GetCredentialDefinitionAsync(Wallet wallet, string credentialDefinitionId);

        /// <summary>
        /// Looks up the credential definition on the ledger.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="definitionId">The identifier of the definition to resolve.</param>
        /// <returns>A json string of the credential definition</returns>
        Task<string> LookupCredentialDefinitionAsync(Pool pool, string definitionId);

        /// <summary>
        /// Looks up the schema definition on the ledger given a credential definition identifier.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="credentialDefinitionId">The credential definition id.</param>
        /// <returns>A json string of the schema</returns>
        Task<string> LookupSchemaFromCredentialDefinitionAsync(Pool pool, string credentialDefinitionId);

        /// <summary>
        /// Looks up the schema definition on the ledger.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="sequenceId">The sequence identifier of the schema to resolve.</param>
        /// <returns>A json string of the schema</returns>
        Task<string> LookupSchemaAsync(Pool pool, int sequenceId);

        /// <summary>
        /// Looks up the schema definition on the ledger.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="schemaId">The identifier of the schema definition to resolve.</param>
        /// <returns>A json string of the schema</returns>
        Task<string> LookupSchemaAsync(Pool pool, string schemaId);
    }
}
