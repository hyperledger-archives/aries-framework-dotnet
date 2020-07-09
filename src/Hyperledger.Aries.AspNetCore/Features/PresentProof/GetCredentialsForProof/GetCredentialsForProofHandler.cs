namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.IssueCredential;
  using Hyperledger.Aries.Features.PresentProof;
  using MediatR;
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetCredentialsForProofHandler : IRequestHandler<GetCredentialsForProofRequest, GetCredentialsForProofResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;

    public GetCredentialsForProofHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
    }
    public async Task<GetCredentialsForProofResponse> Handle
    (
      GetCredentialsForProofRequest aGetCredentialsForProofRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      ProofRecord proofRecord = await ProofService.GetAsync(agentContext, aGetCredentialsForProofRequest.ProofId);

      ProofRequest proofRequest = JsonConvert.DeserializeObject<ProofRequest>(proofRecord.RequestJson);

      //ProofServiceUtils.GetAutoRequestedCredentialsForProofCredentials
      List<Credential> credentials = 
        await ProofService
          .ListCredentialsForProofRequestAsync
          (
            agentContext, 
            proofRequest, 
            aGetCredentialsForProofRequest.Referent
          );
      var response = new GetCredentialsForProofResponse(credentials, aGetCredentialsForProofRequest.CorrelationId);

      return response;
    }
  }
}
