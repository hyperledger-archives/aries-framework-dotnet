namespace BlazorHosted.Features.Healths
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetHealthHandler : IRequestHandler<GetHealthRequest, GetHealthResponse>
  {

    public async Task<GetHealthResponse> Handle
    (
      GetHealthRequest aGetHealthRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetHealthResponse(aGetHealthRequest.Id);

      return await Task.Run(() => response);
    }
  }
}
