using Hyperledger.Aries.Agents;
using System;
using System.Collections.Generic;

namespace Hyperledger.Aries.Features.Statistic.Messages
{
    public class PresentProofStatisticMessage : AgentMessage
    {
        public string ProofId { get; set; }
        public string VerifierDid { get; set; }
        public string HolderDid { get; set; }
        public IEnumerable<string> CredentialDefinitions { get; set; }

        public PresentProofStatisticMessage() : base()
        {
            Id = Guid.NewGuid().ToString();
            Type = MessageTypesHttps.StatisticNames.ProofPresentation;
        }
    }
}
