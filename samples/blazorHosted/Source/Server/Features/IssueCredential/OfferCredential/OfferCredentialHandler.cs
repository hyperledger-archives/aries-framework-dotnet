namespace BlazorHosted.Features.IssueCredentials
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class OfferCredentialHandler : IRequestHandler<OfferCredentialRequest, OfferCredentialResponse>
  {

    public async Task<OfferCredentialResponse> Handle
    (
      OfferCredentialRequest aOfferCredentialRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new OfferCredentialResponse(aOfferCredentialRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
