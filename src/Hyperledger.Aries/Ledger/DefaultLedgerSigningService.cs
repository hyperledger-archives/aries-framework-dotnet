using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using IndyLedger = Hyperledger.Indy.LedgerApi.Ledger;
using Hyperledger.Indy.WalletApi;
using Hyperledger.Aries.Configuration;
using System;

namespace Hyperledger.Aries.Ledger
{
    /// <inheritdoc />
    public class DefaultLedgerSigningService : ILedgerSigningService
    {
        private readonly IProvisioningService provisioningService;

        public DefaultLedgerSigningService(IProvisioningService provisioningService)
        {
            this.provisioningService = provisioningService;
        }
        /// <inheritdoc />
        public async Task<string> SignRequestAsync(IAgentContext context, string submitterDid, string requestJson)
        {
            var provisioning = await provisioningService.GetProvisioningAsync(context.Wallet);

            if (provisioning.TaaAcceptance != null)
            {
                requestJson = await IndyLedger.AppendTxnAuthorAgreementAcceptanceToRequestAsync(requestJson, provisioning.TaaAcceptance.Text,
                    provisioning.TaaAcceptance.Version, provisioning.TaaAcceptance.Digest, "service_agreement", (ulong)DateTimeOffset.Now.ToUnixTimeSeconds());
            }
            return await SignRequestAsync(context.Wallet, submitterDid, requestJson);
        }

        /// <inheritdoc />
        public Task<string> SignRequestAsync(Wallet wallet, string submitterDid, string requestJson)
        {
            return IndyLedger.SignRequestAsync(wallet, submitterDid, requestJson);
        }
    }
}