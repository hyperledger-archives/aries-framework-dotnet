using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Ledger.Models;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Signatures;
using Hyperledger.Aries.Utils;
using indy_vdr_dotnet;
using indy_vdr_dotnet.libindy_vdr;
using Newtonsoft.Json.Linq;

namespace Hyperledger.Aries.Ledger.V2
{
    /// <inheritdoc />
    public class DefaultLedgerServiceV2 : ILedgerService
    {
        private readonly ISigningService _signingService;
        private readonly IPoolService _poolService;
        private readonly IProvisioningService _provisioningService;

        /// <summary>
        /// DefaultLedgerService using Indy-VDR to connect with indy ledgers
        /// </summary>
        /// <param name="signingService"><see cref="ISigningService"/></param>
        /// <param name="poolService"><see cref="IPoolService"/></param>
        /// <param name="provisioningService"><see cref="IProvisioningService"/></param>
        public DefaultLedgerServiceV2(ISigningService signingService, IPoolService poolService, IProvisioningService provisioningService)
        {
            _signingService = signingService;
            _poolService = poolService;
            _provisioningService = provisioningService;
        }

        /// <inheritdoc />
        public Task<IList<AuthorizationRule>> LookupAuthorizationRulesAsync(IAgentContext agentContext)
        {
            throw new NotImplementedException($"{nameof(LookupAuthorizationRulesAsync)} is not implemented");
        }

        /// <inheritdoc />
        public async Task<string> LookupAttributeAsync(IAgentContext agentContext, string targetDid, string attributeName)
        {
            var req = await LedgerApi.BuildGetAttributeRequest(targetDid, null, attributeName, null, null);

            var res = await SubmitRequestAsync(agentContext, req);
            
            var jobj = JObject.Parse(res);
            var data = jobj["result"]?["data"]?.ToString() ?? throw new ArgumentNullException(attributeName);
            
            var result = JObject.Parse(data)[attributeName]?.ToString();
            
            return result;
        }

        /// <inheritdoc />
        public async Task RegisterAttributeAsync(IAgentContext context, string submittedDid, string targetDid, string attributeName,
            object value, TransactionCost paymentInfo = null)
        {
            var data = $"{{\"{attributeName}\": {value.ToJson()}}}";

            var req = await LedgerApi.BuildAttributeRequest(targetDid, submittedDid, null, data);

            await SignAndSubmitRequestAsync(context, submittedDid, req);
        }

        /// <inheritdoc />
        public async Task<ParseResponseResult> LookupSchemaAsync(IAgentContext agentContext, string schemaId)
        {
            var req = await LedgerApi.BuildGetSchemaRequestAsync(schemaId);

            var response = await SubmitRequestAsync(agentContext, req);

            return ResponseParser.ParseGetSchemaResponse(response);
        }

        /// <inheritdoc />
        public async Task<string> LookupNymAsync(IAgentContext agentContext, string did)
        {
            var req = await LedgerApi.BuildGetNymRequest(did);

            return await SubmitRequestAsync(agentContext, req);
        }

        /// <inheritdoc />
        public async Task<string> LookupTransactionAsync(IAgentContext agentContext, string ledgerType, int sequenceId)
        {
            var req = await LedgerApi.BuildGetTxnRequestAsync(Int32.Parse(ledgerType), sequenceId);

            return await SubmitRequestAsync(agentContext, req);
        }

        /// <inheritdoc />
        public async Task<ParseResponseResult> LookupDefinitionAsync(IAgentContext agentContext, string definitionId)
        {
            CredDefId credDefId = new CredDefId(definitionId);
            var response = await LookupTransactionAsync(agentContext, "1", credDefId.SeqNo);
            var jobj = JObject.Parse(response);
            var schemaTxnId = jobj["result"]?["data"]?["txnMetadata"]?["txnId"]?.ToString() ?? throw new ArgumentNullException("schemaTxnId");
            
            var req = await LedgerApi.BuildGetCredDefRequest(definitionId);
            var res = await SubmitRequestAsync(agentContext, req);

            return ResponseParser.ParseGetCredDefResponse(definitionId, schemaTxnId, res);
        }

        /// <inheritdoc />
        public async Task<ParseResponseResult> LookupRevocationRegistryDefinitionAsync(IAgentContext agentContext, string registryId)
        {
            var req = await LedgerApi.BuildGetRevocRegDefRequest(registryId);
            var res = await SubmitRequestAsync(agentContext, req);
            
            return ResponseParser.ParseRegistryDefinitionResponse(registryId, res);
        }

        /// <inheritdoc />
        public async Task<ParseRegistryResponseResult> LookupRevocationRegistryDeltaAsync(IAgentContext agentContext, string revocationRegistryId, long from, long to)
        {
            var req = await LedgerApi.BuildGetRevocRegDeltaRequestAsync(revocationRegistryId, to, from);

            var res = await SubmitRequestAsync(agentContext, req);
            
            return ResponseParser.ParseRevocRegResponse(res);
        }

        /// <inheritdoc />
        public async Task<ParseRegistryResponseResult> LookupRevocationRegistryAsync(IAgentContext agentContext, string revocationRegistryId, long timestamp)
        {
            var req = await LedgerApi.BuildGetRevocRegRequest(revocationRegistryId, timestamp);
            var res = await SubmitRequestAsync(agentContext, req);
            
            return ResponseParser.ParseRevocRegResponse(res);
        }

        /// <inheritdoc />
        public async Task RegisterNymAsync(IAgentContext context, string submitterDid, string theirDid, string theirVerkey, string role,
            TransactionCost paymentInfo = null)
        {
            var req = await LedgerApi.BuildNymRequestAsync(submitterDid, theirDid, theirVerkey, role: role);
            
            await SignAndSubmitRequestAsync(context, submitterDid, req);
        }

        /// <inheritdoc />
        public async Task RegisterCredentialDefinitionAsync(IAgentContext context, string submitterDid, string data,
            TransactionCost paymentInfo = null)
        {
            var req = await LedgerApi.BuildCredDefRequest(submitterDid, data);

            await SignAndSubmitRequestAsync(context, submitterDid, req);
        }

        public async Task RegisterRevocationRegistryDefinitionAsync(IAgentContext context, string submitterDid, string data,
            TransactionCost paymentInfo = null)
        {
            var req = await LedgerApi.BuildRevocRegDefRequestAsync(submitterDid, data);
            
            await SignAndSubmitRequestAsync(context, submitterDid, req);
        }

        /// <inheritdoc />
        public async Task SendRevocationRegistryEntryAsync(IAgentContext context, string issuerDid, string revocationRegistryDefinitionId,
            string revocationDefinitionType, string value, TransactionCost paymentInfo = null)
        {
            var req = await LedgerApi.BuildRevocRegEntryRequestAsync(issuerDid, revocationRegistryDefinitionId,
                revocationDefinitionType, value);
            
            await SignAndSubmitRequestAsync(context, issuerDid, req);
        }

        /// <inheritdoc />
        public async Task RegisterSchemaAsync(IAgentContext context, string issuerDid, string schemaJson,
            TransactionCost paymentInfo = null)
        {
            var req = await LedgerApi.BuildSchemaRequestAsync(issuerDid, schemaJson);
            
            await SignAndSubmitRequestAsync(context, issuerDid, req);
        }

        /// <inheritdoc />
        public async Task<ServiceEndpointResult> LookupServiceEndpointAsync(IAgentContext context, string did)
        {
            var response = await LookupAttributeAsync(context, did, "endpoint");
            
            var endpoint = JObject.Parse(response)["endpoint"]?.ToString();
            
            return new ServiceEndpointResult {Result = new ServiceEndpointResult.ServiceEndpoint {Endpoint = endpoint}};
        }

        /// <inheritdoc />
        public Task RegisterServiceEndpointAsync(IAgentContext context, string did, string serviceEndpoint,
            TransactionCost paymentInfo = null)
        {
            var value = new {endpoint = serviceEndpoint};
            return RegisterAttributeAsync(context, did, did, "endpoint", value);
        }
        
        
        /// <summary>
        /// Adds transaction author agreement, sign and submit the ledger request.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="signingDid">The signing did.</param>
        /// <param name="requestHandle">The request handle.</param>
        /// <returns></returns>
        protected async Task<string> SignAndSubmitRequestAsync(IAgentContext context, string signingDid, IntPtr requestHandle)
        {
            var provisioning = await _provisioningService.GetProvisioningAsync(context.Wallet);
            if (provisioning?.TaaAcceptance != null)
            {
                var agreementAcceptance = await RequestApi.PrepareTxnAuthorAgreementAcceptanceAsync(
                    provisioning.TaaAcceptance.AcceptanceMechanism,
                    (ulong) DateTimeOffset.Now.ToUnixTimeSeconds(),
                    provisioning.TaaAcceptance.Text,
                    provisioning.TaaAcceptance.Version,
                    provisioning.TaaAcceptance.Digest);

                await RequestApi.RequestSetTxnAuthorAgreementAcceptanceAsync(requestHandle, agreementAcceptance);
            }
            
            var unsignedRequest = await RequestApi.RequestGetSignatureInputAsync(requestHandle);
            var signature = await _signingService.SignMessageAsync(context, signingDid, unsignedRequest);
            await RequestApi.RequestSetSigantureAsync(requestHandle, signature);

            return await SubmitRequestAsync(context, requestHandle);
        }

        /// <summary>
        /// Submit the ledger request.
        /// </summary>
        /// <param name="context">The agent context.</param>
        /// <param name="requestHandle">The ledger request.</param>
        /// <returns></returns>
        protected async Task<string> SubmitRequestAsync(IAgentContext context, IntPtr requestHandle)
        {
            async Task<string> SubmitAsync()
            {
                return await _poolService.SubmitRequestAsync(context.Pool, requestHandle);
            }

            return await ResilienceUtils.RetryPolicyAsync(
                action: SubmitAsync,
                exceptionPredicate: (IndyVdrException e) => e.Message.Contains("PoolTimeout") || 
                                                            e.Message.Contains("Service unavailable"));
        }
    }
}
