namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class RemoveCredentialHandler : IRequestHandler<RemoveCredentialRequest, RemoveCredentialResponse>
  {

    public async Task<RemoveCredentialResponse> Handle
    (
      RemoveCredentialRequest aRemoveCredentialRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new RemoveCredentialResponse(aRemoveCredentialRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
