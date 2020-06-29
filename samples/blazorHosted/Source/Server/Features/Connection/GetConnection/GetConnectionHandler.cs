namespace BlazorHosted.Features.Connections
{
  using Hyperledger.Aries.Agents;
  using Hyperledger.Aries.Features.DidExchange;
  using MediatR;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetConnectionHandler : IRequestHandler<GetConnectionRequest, GetConnectionResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IConnectionService ConnectionService;

    public GetConnectionHandler(IAgentProvider aAgentProvider, IConnectionService aConnectionService)
    {
      AgentProvider = aAgentProvider;
      ConnectionService = aConnectionService;
    }

    public async Task<GetConnectionResponse> Handle
    (
      GetConnectionRequest aGetConnectionRequest,
      CancellationToken aCancellationToken
    )
    {

      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      ConnectionRecord connectionRecord = (await ConnectionService.ListAsync(agentContext))
        .FirstOrDefault(aConnectionRecord => aConnectionRecord.Id == aGetConnectionRequest.ConnectionId);
      return new GetConnectionResponse(aGetConnectionRequest.CorrelationId, connectionRecord);
    }
  }
}
