namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Utils;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class AcceptInvitationHandler : IRequestHandler<AcceptInvitationRequest, AcceptInvitationResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IConnectionService ConnectionService;
    private readonly IMessageService MessageService;

    public AcceptInvitationHandler
    (
      IAgentProvider aAgentProvider,
      IConnectionService aConnectionService,
      IMessageService aMessageService
    )
    {
      AgentProvider = aAgentProvider;
      ConnectionService = aConnectionService;
      MessageService = aMessageService;
    }

    public async Task<AcceptInvitationResponse> Handle
    (
      AcceptInvitationRequest aAcceptInvitationRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      ConnectionInvitationMessage connectionInvitationMessage =
        MessageUtils
          .DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(aAcceptInvitationRequest.InvitationDetails);

      (ConnectionRequestMessage connectionRequestMessage, ConnectionRecord connectionRecord)
        = await ConnectionService.CreateRequestAsync(agentContext, connectionInvitationMessage);

      await MessageService.SendAsync(agentContext, connectionRequestMessage, connectionRecord);

      var response = new AcceptInvitationResponse(aAcceptInvitationRequest.CorrelationId);

      return response;
    }
  }
}
