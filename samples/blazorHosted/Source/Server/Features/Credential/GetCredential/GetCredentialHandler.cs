namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialHandler : IRequestHandler<GetCredentialRequest, GetCredentialResponse>
  {

    public async Task<GetCredentialResponse> Handle
    (
      GetCredentialRequest aGetCredentialRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetCredentialResponse(aGetCredentialRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
