namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Agents;
  using Aries.Configuration;
  using Aries.Features.Handshakes.Common;
  using Aries.Features.Handshakes.Connection;
  using Aries.Features.Handshakes.Connection.Models;
  using Extensions;
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
      string encodedInvitation = connectionInvitationMessage.ToJson().ToBase64Url();
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
