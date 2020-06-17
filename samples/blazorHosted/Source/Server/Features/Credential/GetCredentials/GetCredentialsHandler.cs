namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialsHandler : IRequestHandler<GetCredentialsRequest, GetCredentialsResponse>
  {

    public async Task<GetCredentialsResponse> Handle
    (
      GetCredentialsRequest aGetCredentialsRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetCredentialsResponse(aGetCredentialsRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
