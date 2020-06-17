namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class DeleteConnectionHandler : IRequestHandler<DeleteConnectionRequest, DeleteConnectionResponse>
  {

    public async Task<DeleteConnectionResponse> Handle
    (
      DeleteConnectionRequest aDeleteConnectionRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new DeleteConnectionResponse(aDeleteConnectionRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
