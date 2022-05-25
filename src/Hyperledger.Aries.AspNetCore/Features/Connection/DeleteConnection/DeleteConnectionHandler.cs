namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Agents;
  using Aries.Features.Handshakes.Connection;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;

  public class DeleteConnectionHandler : IRequestHandler<DeleteConnectionRequest, DeleteConnectionResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IConnectionService ConnectionService;

    public DeleteConnectionHandler(IAgentProvider aAgentProvider, IConnectionService aConnectionService)
    {
      AgentProvider = aAgentProvider;
      ConnectionService = aConnectionService;
    }

    public async Task<DeleteConnectionResponse> Handle
    (
      DeleteConnectionRequest aDeleteConnectionRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();
      await ConnectionService.DeleteAsync(agentContext, aDeleteConnectionRequest.ConnectionId);

      var response = new DeleteConnectionResponse(aDeleteConnectionRequest.CorrelationId);

      return response;
    }
  }
}
