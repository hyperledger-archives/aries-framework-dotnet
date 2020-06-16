namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class SendPingHandler : IRequestHandler<SendPingRequest, SendPingResponse>
  {

    public async Task<SendPingResponse> Handle
    (
      SendPingRequest aSendPingRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new SendPingResponse(aSendPingRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
