namespace BlazorHosted.Features.PresentProofs
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialsForProofHandler : IRequestHandler<GetCredentialsForProofRequest, GetCredentialsForProofResponse>
  {

    public async Task<GetCredentialsForProofResponse> Handle
    (
      GetCredentialsForProofRequest aGetCredentialsForProofRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetCredentialsForProofResponse(aGetCredentialsForProofRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
