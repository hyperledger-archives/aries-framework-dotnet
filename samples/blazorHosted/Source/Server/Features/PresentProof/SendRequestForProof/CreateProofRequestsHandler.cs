namespace BlazorHosted.Features.PresentProofs
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.PresentProof;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateProofRequestsHandler : IRequestHandler<CreateProofRequestRequest, CreateProofRequestResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;

    public CreateProofRequestsHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
    }

    public async Task<CreateProofRequestResponse> Handle
    (
      CreateProofRequestRequest aSendRequestForProofRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      (RequestPresentationMessage requestPresentationMessage, ProofRecord proofRecord) =
        await ProofService.CreateRequestAsync(agentContext, aSendRequestForProofRequest.ProofRequest, aSendRequestForProofRequest.ConnectionId);

      var response = new CreateProofRequestResponse(requestPresentationMessage, aSendRequestForProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
