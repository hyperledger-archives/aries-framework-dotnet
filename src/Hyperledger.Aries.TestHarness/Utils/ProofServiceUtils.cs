using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.PresentProof;

namespace Hyperledger.TestHarness.Utils
{
    public static class ProofServiceUtils
    {
        public static async Task<RequestedCredentials> GetAutoRequestedCredentialsForProofCredentials(IAgentContext holderContext,
            IProofService proofService, ProofRequest proofRequest)
        {
            var requestedCredentials = new RequestedCredentials();
            foreach (var requestedAttribute in proofRequest.RequestedAttributes)
            {
                var credentials =
                    await proofService.ListCredentialsForProofRequestAsync(holderContext, proofRequest,
                        requestedAttribute.Key);

                requestedCredentials.RequestedAttributes.Add(requestedAttribute.Key,
                    new RequestedAttribute
                    {
                        CredentialId = credentials.First().CredentialInfo.Referent,
                        Revealed = true
                    });
            }

            foreach (var requestedAttribute in proofRequest.RequestedPredicates)
            {
                var credentials =
                    await proofService.ListCredentialsForProofRequestAsync(holderContext, proofRequest,
                        requestedAttribute.Key);

                requestedCredentials.RequestedPredicates.Add(requestedAttribute.Key,
                    new RequestedAttribute
                    {
                        CredentialId = credentials.First().CredentialInfo.Referent,
                        Revealed = true
                    });
            }

            return requestedCredentials;
        }
    }
}
