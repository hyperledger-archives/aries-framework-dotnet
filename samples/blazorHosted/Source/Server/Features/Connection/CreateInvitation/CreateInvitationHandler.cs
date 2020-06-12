namespace BlazorHosted.Features.Connections
{
  using AutoMapper;
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.DidExchange;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class CreateInvitationHandler : IRequestHandler<CreateInvitationRequest, CreateInvitationResponse>
  {
    public CreateInvitationHandler(IMapper aMapper, IAgentProvider aAgentProvider, IConnectionService aConnectionService)
    {
      Mapper = aMapper;
      AgentProvider = aAgentProvider;
      ConnectionService = aConnectionService;
    }

    public IMapper Mapper { get; }
    public IAgentProvider AgentProvider { get; }
    public IConnectionService ConnectionService { get; }

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
        }
      };

      (ConnectionInvitationMessage connectionInvitationMessage, ConnectionRecord connectionRecord) =
        await ConnectionService.CreateInvitationAsync(agentContext, inviteConfiguration);
      InvitationDto invitationDto = Mapper.Map<InvitationDto>(connectionInvitationMessage);
      var response = new CreateInvitationResponse(aCreateInvitationRequest.RequestId, invitationDto);

      return response;
    }
  }
}
