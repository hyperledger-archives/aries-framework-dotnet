namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.PresentProof;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetProofHandler : IRequestHandler<GetProofRequest, GetProofResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;

    public GetProofHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
    }

    public async Task<GetProofResponse> Handle
    (
      GetProofRequest aGetProofRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      ProofRecord proofRecord = await ProofService.GetAsync(agentContext, aGetProofRequest.ProofId);
      var response = new GetProofResponse(proofRecord, aGetProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
