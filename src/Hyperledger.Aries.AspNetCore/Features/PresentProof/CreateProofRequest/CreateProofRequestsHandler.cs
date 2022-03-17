namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Agents;
  using Aries.Configuration;
  using Aries.Features.Handshakes.Common;
  using Aries.Features.Handshakes.Connection;
  using Aries.Features.PresentProof;
  using Indy.AnonCredsApi;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateProofRequestsHandler : IRequestHandler<CreateProofRequestRequest, CreateProofRequestResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IProofService ProofService;
    private readonly IProvisioningService ProvisioningService;
    private readonly IConnectionService ConnectionService;
    private readonly IMessageService MessageService;

    public CreateProofRequestsHandler
    (
      IAgentProvider aAgentProvider,
      IProofService aProofService,
      IProvisioningService aProvisioningService,
      IConnectionService aConnectionService,
      IMessageService aMessageService
    )
    {
      AgentProvider = aAgentProvider;
      ProofService = aProofService;
      ProvisioningService = aProvisioningService;
      ConnectionService = aConnectionService;
      MessageService = aMessageService;
    }

    public async Task<CreateProofRequestResponse> Handle
    (
      CreateProofRequestRequest aCreateProofRequestRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      ConnectionRecord connectionRecord = await ConnectionService.GetAsync(agentContext, aCreateProofRequestRequest.ConnectionId);     
      aCreateProofRequestRequest.ProofRequest.Nonce = await AnonCreds.GenerateNonceAsync();

      (RequestPresentationMessage requestPresentationMessage, ProofRecord proofRecord) =
        await ProofService.CreateRequestAsync(agentContext, aCreateProofRequestRequest.ProofRequest, aCreateProofRequestRequest.ConnectionId);

      await MessageService.SendAsync(agentContext, requestPresentationMessage, connectionRecord);

      //(requestPresentationMessage, proofRecord) =
      //  await ProofService.CreateRequestAsync(agentContext, aSendRequestForProofRequest.ProofRequest);   
      
      //string endpointUri = (await ProvisioningService.GetProvisioningAsync(agentContext.Wallet)).Endpoint.Uri;
      //string encodedRequestPresentationMessage = requestPresentationMessage.ToJson().ToBase64();
      //string proofRequestUrl = $"{endpointUri}?c_i={encodedRequestPresentationMessage}";

      var response = new CreateProofRequestResponse(requestPresentationMessage, aCreateProofRequestRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
