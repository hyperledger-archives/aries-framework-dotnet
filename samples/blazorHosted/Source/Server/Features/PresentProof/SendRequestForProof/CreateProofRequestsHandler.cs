namespace BlazorHosted.Features.PresentProofs
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Features.PresentProof;
  using Hyperledger.Aries.Extensions;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateProofRequestsHandler : IRequestHandler<CreateProofRequestRequest, CreateProofRequestResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;
    private readonly IProvisioningService ProvisioningService;

    public CreateProofRequestsHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService,
      IProvisioningService aProvisioningService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
      ProvisioningService = aProvisioningService;
    }

    public async Task<CreateProofRequestResponse> Handle
    (
      CreateProofRequestRequest aSendRequestForProofRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      (RequestPresentationMessage requestPresentationMessage, ProofRecord proofRecord) =
        await ProofService.CreateRequestAsync(agentContext, aSendRequestForProofRequest.ProofRequest);
      
      string endpointUri = (await ProvisioningService.GetProvisioningAsync(agentContext.Wallet)).Endpoint.Uri;
      string encodedRequestPresentationMessage = requestPresentationMessage.ToJson().ToBase64();
      string proofRequestUrl = $"{endpointUri}?c_i={encodedRequestPresentationMessage}";

      var response = new CreateProofRequestResponse(requestPresentationMessage, proofRequestUrl, aSendRequestForProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
