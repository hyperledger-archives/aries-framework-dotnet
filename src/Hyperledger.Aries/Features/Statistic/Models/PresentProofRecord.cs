using Hyperledger.Aries.Storage;
using System.Collections.Generic;

namespace Hyperledger.Aries.Features.Statistic.Models
{
    public class PresentProofRecord : RecordBase
    {
        public string ProofId { get; set; }
        public string VerifierDid { get; set; }
        public string HolderDid { get; set; }
        public IEnumerable<string> CredentialDefinitions { get; set; }
        public override string TypeName => "AF.PresentProofRecord";
    }
}
