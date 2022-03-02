namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Agents;
  using Aries.Features.Handshakes.Common;
  using Aries.Features.Handshakes.Connection;
  using MediatR;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;

  public class GetConnectionsHandler : IRequestHandler<GetConnectionsRequest, GetConnectionsResponse>
  {
    private readonly IAgentProvider AgentProvider;
    private readonly IConnectionService ConnectionService;

    public GetConnectionsHandler(IAgentProvider aAgentProvider, IConnectionService aConnectionService)
    {
      AgentProvider = aAgentProvider;
      ConnectionService = aConnectionService;
    }

    public async Task<GetConnectionsResponse> Handle
    (
      GetConnectionsRequest aGetConnectionsRequest,
      CancellationToken aCancellationToken
    )
    {
      IAgentContext agentContext = await AgentProvider.GetContextAsync();

      List<ConnectionRecord> connections = await ConnectionService.ListAsync(agentContext);
      return new GetConnectionsResponse(aGetConnectionsRequest.CorrelationId, connections);
    }
  }
}
