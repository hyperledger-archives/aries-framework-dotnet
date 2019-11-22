using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using IndyLedger = Hyperledger.Indy.LedgerApi.Ledger;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Ledger
{
    /// <inheritdoc />
    public class DefaultLedgerSigningService : ILedgerSigningService
    {
        /// <inheritdoc />
        public Task<string> SignRequestAsync(IAgentContext context, string submitterDid, string requestJson)
            => SignRequestAsync(context.Wallet, submitterDid, requestJson);

        /// <inheritdoc />
        public Task<string> SignRequestAsync(Wallet wallet, string submitterDid, string requestJson)
            => IndyLedger.SignRequestAsync(wallet, submitterDid, requestJson);
    }
}