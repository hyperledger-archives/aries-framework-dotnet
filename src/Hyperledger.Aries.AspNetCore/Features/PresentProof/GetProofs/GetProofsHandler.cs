namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.PresentProof;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetProofsHandler : IRequestHandler<GetProofsRequest, GetProofsResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;

    public GetProofsHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
    }

    public async Task<GetProofsResponse> Handle
    (
      GetProofsRequest aGetProofsRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      List<ProofRecord> proofRecords = await ProofService.ListAsync(agentContext);

      var response = new GetProofsResponse(proofRecords, aGetProofsRequest.CorrelationId);
      
      return response;
    }
  }
}
