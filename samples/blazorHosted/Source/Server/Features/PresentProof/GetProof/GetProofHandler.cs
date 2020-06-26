namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetProofHandler : IRequestHandler<GetProofRequest, GetProofResponse>
  {

    public async Task<GetProofResponse> Handle
    (
      GetProofRequest aGetProofRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetProofResponse(aGetProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
