using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using Hyperledger.Indy.AnonCredsApi;
using Microsoft.Extensions.Logging;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public partial class DefaultCredentialService : ICredentialService
    {
        /// <summary>
        /// The event aggregator.
        /// </summary>
        protected readonly IEventAggregator EventAggregator;

        /// <summary>
        /// The ledger service
        /// </summary>
        protected readonly ILedgerService LedgerService;
        /// <summary>
        /// The connection service
        /// </summary>
        protected readonly IConnectionService ConnectionService;
        /// <summary>
        /// The record service
        /// </summary>
        protected readonly IWalletRecordService RecordService;
        /// <summary>
        /// The schema service
        /// </summary>
        protected readonly ISchemaService SchemaService;
        /// <summary>
        /// The tails service
        /// </summary>
        protected readonly ITailsService TailsService;
        /// <summary>
        /// The provisioning service
        /// </summary>
        protected readonly IProvisioningService ProvisioningService;

        /// <summary>
        /// Payment Service
        /// </summary>
        protected readonly IPaymentService PaymentService;

        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger<DefaultCredentialService> Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCredentialService"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="ledgerService">The ledger service.</param>
        /// <param name="connectionService">The connection service.</param>
        /// <param name="recordService">The record service.</param>
        /// <param name="schemaService">The schema service.</param>
        /// <param name="tailsService">The tails service.</param>
        /// <param name="provisioningService">The provisioning service.</param>
        /// <param name="paymentService">The payment service.</param>
        /// <param name="logger">The logger.</param>
        public DefaultCredentialService(
            IEventAggregator eventAggregator,
            ILedgerService ledgerService,
            IConnectionService connectionService,
            IWalletRecordService recordService,
            ISchemaService schemaService,
            ITailsService tailsService,
            IProvisioningService provisioningService,
            IPaymentService paymentService,
            ILogger<DefaultCredentialService> logger)
        {
            EventAggregator = eventAggregator;
            LedgerService = ledgerService;
            ConnectionService = connectionService;
            RecordService = recordService;
            SchemaService = schemaService;
            TailsService = tailsService;
            ProvisioningService = provisioningService;
            PaymentService = paymentService;
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual async Task<CredentialRecord> GetAsync(IAgentContext agentContext, string credentialId)
        {
            var record = await RecordService.GetAsync<CredentialRecord>(agentContext.Wallet, credentialId);

            if (record == null)
                throw new AgentFrameworkException(ErrorCode.RecordNotFound, "Credential record not found");

            return record;
        }

        /// <inheritdoc />
        public virtual Task<List<CredentialRecord>> ListAsync(IAgentContext agentContext, ISearchQuery query = null, int count = 100) =>
            RecordService.SearchAsync<CredentialRecord>(agentContext.Wallet, query, null, count);

        /// <inheritdoc />
        public virtual async Task RejectOfferAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credential.State}'");

            await credential.TriggerAsync(CredentialTrigger.Reject);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
        }

        /// <inheritdoc />
        public async Task RevokeCredentialOfferAsync(IAgentContext agentContext, string offerId)
        {
            var credentialRecord = await GetAsync(agentContext, offerId);

            if (credentialRecord.State != CredentialState.Offered)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Offered}', found '{credentialRecord.State}'");

            await RecordService.DeleteAsync<ConnectionRecord>(agentContext.Wallet, offerId);
        }

        /// <inheritdoc />
        public virtual async Task RejectCredentialRequestAsync(IAgentContext agentContext, string credentialId)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Requested)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Requested}', found '{credential.State}'");

            await credential.TriggerAsync(CredentialTrigger.Reject);
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
        }

        /// <inheritdoc />
        public virtual async Task RevokeCredentialAsync(IAgentContext agentContext, string credentialId, string issuerDid)
        {
            var credential = await GetAsync(agentContext, credentialId);

            if (credential.State != CredentialState.Issued)
                throw new AgentFrameworkException(ErrorCode.RecordInInvalidState,
                    $"Credential state was invalid. Expected '{CredentialState.Requested}', found '{credential.State}'");

            var definition = await SchemaService.GetCredentialDefinitionAsync(agentContext.Wallet, credential.CredentialDefinitionId);

            // Check if the state machine is valid for revocation
            await credential.TriggerAsync(CredentialTrigger.Revoke);

            var revocationRecordSearch = await RecordService.SearchAsync<RevocationRegistryRecord>(
                agentContext.Wallet, SearchQuery.Equal(nameof(RevocationRegistryRecord.CredentialDefinitionId), definition.Id), null, 5);
            var revocationRecord = revocationRecordSearch.Single(); // TODO: Add support for multiple revocation registries

            // Revoke the credential
            var tailsReader = await TailsService.OpenTailsAsync(revocationRecord.TailsFile);
            var revocRegistryDeltaJson = await AnonCreds.IssuerRevokeCredentialAsync(agentContext.Wallet, tailsReader,
                revocationRecord.Id, credential.CredentialRevocationId);

            var paymentInfo = await PaymentService.GetTransactionCostAsync(agentContext, TransactionTypes.REVOC_REG_ENTRY);

            // Write the delta state on the ledger for the corresponding revocation registry
            await LedgerService.SendRevocationRegistryEntryAsync(context: agentContext,
                                                                 issuerDid: issuerDid,
                                                                 revocationRegistryDefinitionId: revocationRecord.Id,
                                                                 revocationDefinitionType: "CL_ACCUM",
                                                                 value: revocRegistryDeltaJson,
                                                                 paymentInfo: paymentInfo);

            if (paymentInfo != null)
            {
                await RecordService.UpdateAsync(agentContext.Wallet, paymentInfo.PaymentAddress);
            }

            // Update local credential record
            await RecordService.UpdateAsync(agentContext.Wallet, credential);
        }

        /// <inheritdoc />
        public async Task DeleteCredentialAsync(IAgentContext agentContext, string credentialId)
        {
            var credentialRecord = await GetAsync(agentContext, credentialId);
            try
            {
                await AnonCreds.ProverDeleteCredentialAsync(agentContext.Wallet, credentialRecord.CredentialId);
            }
            catch
            {
                // OK
            }
            await RecordService.DeleteAsync<CredentialRecord>(agentContext.Wallet, credentialId);
        }
    }
}