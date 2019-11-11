using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Models.Ledger;
using AgentFramework.Core.Models.Payments;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.PoolApi;
using Hyperledger.Indy.WalletApi;
using Newtonsoft.Json.Linq;
using IndyPayments = Hyperledger.Indy.PaymentsApi.Payments;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultLedgerService : ILedgerService
    {
        private readonly ILedgerSigningService _signingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLedgerService" /> class
        /// </summary>
        /// <param name="signingService"></param>
        public DefaultLedgerService(ILedgerSigningService signingService)
        {
            _signingService = signingService;
        }

        /// <inheritdoc />
        public virtual async Task<ParseResponseResult> LookupDefinitionAsync(Pool pool,
            string definitionId)
        {
            var req = await Ledger.BuildGetCredDefRequestAsync(null, definitionId);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            return await Ledger.ParseGetCredDefResponseAsync(res);
        }

        /// <inheritdoc />
        public virtual async Task<ParseResponseResult> LookupRevocationRegistryDefinitionAsync(Pool pool,
            string registryId)
        {
            var req = await Ledger.BuildGetRevocRegDefRequestAsync(null, registryId);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            return await Ledger.ParseGetRevocRegDefResponseAsync(res);
        }

        /// <inheritdoc />
        public virtual async Task<ParseResponseResult> LookupSchemaAsync(Pool pool, string schemaId)
        {
            var req = await Ledger.BuildGetSchemaRequestAsync(null, schemaId);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            return await Ledger.ParseGetSchemaResponseAsync(res);
        }

        /// <inheritdoc />
        public virtual async Task<ParseRegistryResponseResult> LookupRevocationRegistryDeltaAsync(Pool pool, string revocationRegistryId,
             long from, long to)
        {
            var req = await Ledger.BuildGetRevocRegDeltaRequestAsync(null, revocationRegistryId, from, to);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            return await Ledger.ParseGetRevocRegDeltaResponseAsync(res);
        }

        /// <inheritdoc />
        public virtual async Task<ParseRegistryResponseResult> LookupRevocationRegistryAsync(Pool pool, string revocationRegistryId,
             long timestamp)
        {
            var req = await Ledger.BuildGetRevocRegRequestAsync(null, revocationRegistryId, timestamp);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            return await Ledger.ParseGetRevocRegResponseAsync(res);
        }

        /// <inheritdoc />
        public virtual async Task RegisterSchemaAsync(IAgentContext context, string issuerDid, string schemaJson, TransactionCost paymentInfo = null)
        {
            var req = await Ledger.BuildSchemaRequestAsync(issuerDid, schemaJson);
            var res = await SignAndSubmitAsync(context, issuerDid, req, paymentInfo);
        }

        /// <inheritdoc />
        public virtual async Task RegisterCredentialDefinitionAsync(IAgentContext context, string submitterDid, string data, TransactionCost paymentInfo = null)
        {
            var req = await Ledger.BuildCredDefRequestAsync(submitterDid, data);
            var res = await SignAndSubmitAsync(context, submitterDid, req, paymentInfo);
        }

        /// <inheritdoc />
        public virtual async Task RegisterRevocationRegistryDefinitionAsync(IAgentContext context, string submitterDid,
            string data, TransactionCost paymentInfo = null)
        {
            var req = await Ledger.BuildRevocRegDefRequestAsync(submitterDid, data);
            var res = await SignAndSubmitAsync(context, submitterDid, req, paymentInfo);
        }

        /// <inheritdoc />
        public virtual async Task SendRevocationRegistryEntryAsync(IAgentContext context, string issuerDid,
            string revocationRegistryDefinitionId, string revocationDefinitionType, string value, TransactionCost paymentInfo = null)
        {
            var req = await Ledger.BuildRevocRegEntryRequestAsync(issuerDid, revocationRegistryDefinitionId,
                revocationDefinitionType, value);
            var res = await SignAndSubmitAsync(context, issuerDid, req, paymentInfo);
        }

        /// <inheritdoc />
        public virtual async Task RegisterNymAsync(IAgentContext context, string submitterDid, string theirDid,
            string theirVerkey, string role, TransactionCost paymentInfo = null)
        {
            if (DidUtils.IsFullVerkey(theirVerkey))
                theirVerkey = await Did.AbbreviateVerkeyAsync(theirDid, theirVerkey);

            var req = await Ledger.BuildNymRequestAsync(submitterDid, theirDid, theirVerkey, null, role);
            var res = await SignAndSubmitAsync(context, submitterDid, req, paymentInfo);
        }

        /// <inheritdoc />
        public virtual async Task<string> LookupAttributeAsync(Pool pool, string targetDid, string attributeName)
        {
            var req = await Ledger.BuildGetAttribRequestAsync(null, targetDid, attributeName, null, null);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            return null;
        }

        /// <inheritdoc />
        public virtual async Task<string> LookupTransactionAsync(Pool pool, string ledgerType, int sequenceId)
        {
            var req = await Ledger.BuildGetTxnRequestAsync(null, ledgerType, sequenceId);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            return res;
        }

        /// <inheritdoc />
        public virtual async Task RegisterAttributeAsync(IAgentContext context, string submittedDid, string targetDid,
            string attributeName, object value, TransactionCost paymentInfo = null)
        {
            var data = $"{{\"{attributeName}\": {value.ToJson()}}}";

            var req = await Ledger.BuildAttribRequestAsync(submittedDid, targetDid, null, data, null);
            var res = await SignAndSubmitAsync(context, submittedDid, req, paymentInfo);
        }

        /// <inheritdoc />
        public async Task<string> LookupNymAsync(Pool pool, string did)
        {
            var req = await Ledger.BuildGetNymRequestAsync(null, did);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            return res;
        }

        /// <inheritdoc />
        public async Task<IList<AuthorizationRule>> LookupAuthorizationRulesAsync(Pool pool)
        {
            var req = await Ledger.BuildGetAuthRuleRequestAsync(null, null, null, null, null, null);
            var res = await Ledger.SubmitRequestAsync(pool, req);

            EnsureSuccessResponse(res);

            var jobj = JObject.Parse(res);
            return jobj["result"]["data"].ToObject<IList<AuthorizationRule>>();
        }

        private async Task<string> SignAndSubmitAsync(IAgentContext context, string submitterDid, string request, TransactionCost paymentInfo)
        {
            if (paymentInfo != null)
            {
                var requestWithFees = await IndyPayments.AddRequestFeesAsync(
                    wallet: context.Wallet,
                    submitterDid: null,
                    reqJson: request,
                    inputsJson: paymentInfo.PaymentAddress.Sources.Select(x => x.Source).ToJson(),
                    outputsJson: new[]
                    {
                        new IndyPaymentOutputSource
                        {
                            Recipient = paymentInfo.PaymentAddress.Address,
                            Amount = paymentInfo.PaymentAddress.Balance - paymentInfo.Amount
                        }
                    }.ToJson(),
                    extra: null);
                request = requestWithFees.Result;
            }
            var signedRequest = await _signingService.SignRequestAsync(context, submitterDid, request);
            var response = await Ledger.SubmitRequestAsync(await context.Pool, signedRequest);

            EnsureSuccessResponse(response);

            if (paymentInfo != null)
            {
                var responsePayment = await IndyPayments.ParseResponseWithFeesAsync(paymentInfo.PaymentMethod, response);
                var paymentOutputs = responsePayment.ToObject<IList<IndyPaymentOutputSource>>();
                paymentInfo.PaymentAddress.Sources = paymentOutputs
                    .Where(x => x.Recipient == paymentInfo.PaymentAddress.Address)
                    .Select(x => new IndyPaymentInputSource
                    {
                        Amount = x.Amount,
                        PaymentAddress = x.Recipient,
                        Source = x.Receipt
                    })
                    .ToList();
            }
            return response;
        }

        void EnsureSuccessResponse(string res)
        {
            var response = JObject.Parse(res);

            if (!response["op"].ToObject<string>().Equals("reply", StringComparison.OrdinalIgnoreCase))
                throw new AgentFrameworkException(ErrorCode.LedgerOperationRejected, "Ledger operation rejected");
        }
    }
}