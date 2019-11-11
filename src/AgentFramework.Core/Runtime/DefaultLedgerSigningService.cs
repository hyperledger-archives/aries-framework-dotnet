using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    public class DefaultLedgerSigningService : ILedgerSigningService
    {
        /// <inheritdoc />
        public Task<string> SignRequestAsync(IAgentContext context, string submitterDid, string requestJson) 
            => SignRequestAsync(context.Wallet, submitterDid, requestJson);

        /// <inheritdoc />
        public Task<string> SignRequestAsync(Wallet wallet, string submitterDid, string requestJson) 
            => Ledger.SignRequestAsync(wallet, submitterDid, requestJson);
    }
}