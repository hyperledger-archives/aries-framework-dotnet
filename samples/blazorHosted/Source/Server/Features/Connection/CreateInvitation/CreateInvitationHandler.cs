namespace BlazorHosted.Features.Connections
{
  using AutoMapper;
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Configuration;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Extensions;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateInvitationHandler : IRequestHandler<CreateInvitationRequest, CreateInvitationResponse>
  {
    private readonly IMapper Mapper;
    private readonly IAgentProvider AgentProvider;
    private readonly IConnectionService ConnectionService;
    private readonly IProvisioningService ProvisioningService;

    public CreateInvitationHandler
    (
      IMapper aMapper, 
      IAgentProvider aAgentProvider, 
      IConnectionService aConnectionService,
      IProvisioningService aProvisioningService
    )
    {
      Mapper = aMapper;
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

      var inviteConfiguration = new InviteConfiguration
      {
        MyAlias = new ConnectionAlias()
        {
          Name = aCreateInvitationRequest.Alias,
          ImageUrl = aCreateInvitationRequest.ImageUrl?.AbsoluteUri
        },
        AutoAcceptConnection = true
      };
      (ConnectionInvitationMessage connectionInvitationMessage, _) =
        await ConnectionService.CreateInvitationAsync(agentContext, inviteConfiguration);

      string endpointUri = (await ProvisioningService.GetProvisioningAsync(agentContext.Wallet)).Endpoint.Uri;
      string encodedInvitation = connectionInvitationMessage.ToJson().ToBase64();
      var response = new CreateInvitationResponse(aCreateInvitationRequest.CorrelationId, connectionInvitationMessage)
      {
        InvitationUrl = $"{endpointUri}?c_i={encodedInvitation}"
      };

      return response;
    }
  }
}
