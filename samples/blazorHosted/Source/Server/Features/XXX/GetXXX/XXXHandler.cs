namespace BlazorHosted.Features.XXXs
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class XXXHandler : IRequestHandler<XXXRequest, XXXResponse>
  {

    public async Task<XXXResponse> Handle
    (
      XXXRequest aXXXRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new XXXResponse(aXXXRequest.Id);

      return await Task.Run(() => response);
    }
  }
}
