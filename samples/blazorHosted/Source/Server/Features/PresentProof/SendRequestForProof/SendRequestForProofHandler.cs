namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class SendRequestForProofHandler : IRequestHandler<SendRequestForProofRequest, SendRequestForProofResponse>
  {

    public async Task<SendRequestForProofResponse> Handle
    (
      SendRequestForProofRequest aSendRequestForProofRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new SendRequestForProofResponse(aSendRequestForProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
