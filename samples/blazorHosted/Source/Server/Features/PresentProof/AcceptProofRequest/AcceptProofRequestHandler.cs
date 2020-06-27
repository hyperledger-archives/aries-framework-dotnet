namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class AcceptProofRequestHandler : IRequestHandler<AcceptProofRequestRequest, AcceptProofRequestResponse>
  {

    public async Task<AcceptProofRequestResponse> Handle
    (
      AcceptProofRequestRequest aAcceptProofRequestRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new AcceptProofRequestResponse(aAcceptProofRequestRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
