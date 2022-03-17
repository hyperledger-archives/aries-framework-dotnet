namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Agents;
  using Aries.Configuration;
  using Aries.Features.PresentProof;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;
  using Utils;

  public class AcceptProofRequestHandler : IRequestHandler<AcceptProofRequestRequest, AcceptProofRequestResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IMessageService MessageService;
    private readonly IProofService ProofService;
    private readonly IProvisioningService ProvisioningService;

    public AcceptProofRequestHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService,
      IProvisioningService aProvisioningService,
      IMessageService aMessageService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
      ProvisioningService = aProvisioningService;
      MessageService = aMessageService;
    }

    public async Task<AcceptProofRequestResponse> Handle
    (
      AcceptProofRequestRequest aAcceptProofRequestRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      RequestPresentationMessage requestPresentationMessage =
        MessageUtils
          .DecodeMessageFromUrlFormat<RequestPresentationMessage>(aAcceptProofRequestRequest.EncodedProofRequestMessage);

      //ProofRecord proofRecord = 
      //  await ProofService.
      //  CreatePresentationAsync(agentContext, requestPresentationMessage, requestedCredentials: null);

      //await MessageService.SendAsync(agentContext.Wallet, requestPresentationMessage, proofRecord);
      var response = new AcceptProofRequestResponse(aAcceptProofRequestRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
