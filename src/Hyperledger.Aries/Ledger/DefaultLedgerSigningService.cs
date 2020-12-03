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
            try
            {
                var provisioning = await provisioningService.GetProvisioningAsync(context.Wallet);

                if (provisioning?.TaaAcceptance != null)
                {
                    requestJson = await IndyLedger.AppendTxnAuthorAgreementAcceptanceToRequestAsync(requestJson, provisioning.TaaAcceptance.Text,
                        provisioning.TaaAcceptance.Version, provisioning.TaaAcceptance.Digest, provisioning.TaaAcceptance.AcceptanceMechanism, (ulong)DateTimeOffset.Now.ToUnixTimeSeconds());
                }
            }
            catch (AriesFrameworkException ex) when (ex.ErrorCode == ErrorCode.RecordNotFound)
            {
                // OK, used in unit tests and scenarios when we want to simply send ledger commands
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
