namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class SendMessageHandler : IRequestHandler<SendMessageRequest, SendMessageResponse>
  {

    public async Task<SendMessageResponse> Handle
    (
      SendMessageRequest aSendMessageRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new SendMessageResponse(aSendMessageRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
