using System.Text;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Indy.CryptoApi;
using Hyperledger.Indy.DidApi;

namespace Hyperledger.Aries.Signatures
{
    public class DefaultSigningService : ISigningService
    {
        public virtual Task<byte[]> SignMessageAsync(IAgentContext context, string signingDid, string message)
        {
            return SignMessageAsync(context, signingDid, Encoding.UTF8.GetBytes(message));
        }
        
        public virtual async Task<byte[]> SignMessageAsync(IAgentContext context, string signingDid, byte[] message)
        {
            var key = await Did.KeyForLocalDidAsync(context.Wallet, signingDid);
            return await Crypto.SignAsync(context.Wallet, key, message);
        }
    }
}
