using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Indy.WalletApi;

namespace Hyperledger.Aries.Contracts
{
    /// <summary>
    /// Ledger Signing Service
    /// </summary>
    public interface ILedgerSigningService
    {
        /// <summary>
        /// Signs the outgoing request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="submitterDid"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        Task<string> SignRequestAsync(IAgentContext context, string submitterDid, string requestJson);

        /// <summary>
        /// Signs the outgoing request
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="submitterDid"></param>
        /// <param name="requestJson"></param>
        /// <returns></returns>
        Task<string> SignRequestAsync(Wallet wallet, string submitterDid, string requestJson);
    }
}