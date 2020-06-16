namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetConnectionHandler : IRequestHandler<GetConnectionRequest, GetConnectionResponse>
  {

    public async Task<GetConnectionResponse> Handle
    (
      GetConnectionRequest aGetConnectionRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetConnectionResponse(aGetConnectionRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
