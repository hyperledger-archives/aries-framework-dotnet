using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using Hyperledger.Indy.LedgerApi;
using Hyperledger.Indy.WalletApi;

namespace AgentFramework.Core.Runtime
{
    public class DefaultLedgerSigningService : ILedgerSigningService
    {
        public Task<string> SignRequestAsync(IAgentContext context, string submitterDid, string requestJson) 
            => SignRequestAsync(context.Wallet, submitterDid, requestJson);

        public Task<string> SignRequestAsync(Wallet wallet, string submitterDid, string requestJson) 
            => Ledger.SignRequestAsync(wallet, submitterDid, requestJson);
    }
}