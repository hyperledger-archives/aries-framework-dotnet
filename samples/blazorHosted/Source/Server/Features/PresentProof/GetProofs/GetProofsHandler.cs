namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetProofsHandler : IRequestHandler<GetProofsRequest, GetProofsResponse>
  {

    public async Task<GetProofsResponse> Handle
    (
      GetProofsRequest aGetProofsRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetProofsResponse(aGetProofsRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
