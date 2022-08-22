using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Signatures
{
    public interface ISigningService
    {
        public Task<byte[]> SignMessageAsync(IAgentContext context, string signingDid, byte[] message);
        
        public Task<byte[]> SignMessageAsync(IAgentContext context, string signingDid, string message);
    }
}
