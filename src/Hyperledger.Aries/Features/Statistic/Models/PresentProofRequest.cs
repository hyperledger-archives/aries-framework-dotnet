using System.Collections.Generic;

namespace Hyperledger.Aries.Features.Statistic.Models
{
    public class PresentProofRequest
    {
        public string ProofId { get; set; }
        public string VerifierDid { get; set; }
        public string HolderDid { get; set; }
        public IEnumerable<string> CredentialDefinitions { get; set; }
    }
}
