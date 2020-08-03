namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Extensions;
  using Hyperledger.Aries.Features.DidExchange;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateInvitationHandler : IRequestHandler<CreateInvitationRequest, CreateInvitationResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IConnectionService ConnectionService;
    private readonly IProvisioningService ProvisioningService;

    public CreateInvitationHandler
    (
      IAgentProvider aAgentProvider,
      IConnectionService aConnectionService,
      IProvisioningService aProvisioningService
    )
    {
      AgentProvider = aAgentProvider;
      ConnectionService = aConnectionService;
      ProvisioningService = aProvisioningService;
    }

    public async Task<CreateInvitationResponse> Handle
    (
      CreateInvitationRequest aCreateInvitationRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      (ConnectionInvitationMessage connectionInvitationMessage, ConnectionRecord connectionRecord) =
        await ConnectionService.CreateInvitationAsync(agentContext, aCreateInvitationRequest.InviteConfiguration);

      string endpointUri = (await ProvisioningService.GetProvisioningAsync(agentContext.Wallet)).Endpoint.Uri;
      string encodedInvitation = connectionInvitationMessage.ToJson().ToBase64();
      var response =
        new CreateInvitationResponse
        (
          aCreateInvitationRequest.CorrelationId,
          connectionInvitationMessage,
          connectionRecord,
          aInvitationUrl: $"{endpointUri}?c_i={encodedInvitation}"
        );

      return response;
    }
  }
}
