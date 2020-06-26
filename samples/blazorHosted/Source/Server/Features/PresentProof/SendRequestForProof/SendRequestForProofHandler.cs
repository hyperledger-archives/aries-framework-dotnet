namespace BlazorHosted.Features.PresentProofs
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.PresentProof;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class SendRequestForProofHandler : IRequestHandler<SendRequestForProofRequest, SendRequestForProofResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;

    public SendRequestForProofHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
    }

    public async Task<SendRequestForProofResponse> Handle
    (
      SendRequestForProofRequest aSendRequestForProofRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      (RequestPresentationMessage requestPresentationMessage, ProofRecord proofRecord) = 
        await ProofService.CreateRequestAsync(agentContext, aSendRequestForProofRequest.ProofRequest);

      var response = new SendRequestForProofResponse(requestPresentationMessage, aSendRequestForProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
