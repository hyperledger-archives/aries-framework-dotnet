namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialDefinitionsHandler : IRequestHandler<GetCredentialDefinitionsRequest, GetCredentialDefinitionsResponse>
  {

    public async Task<GetCredentialDefinitionsResponse> Handle
    (
      GetCredentialDefinitionsRequest aGetCredentialDefinitionsRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetCredentialDefinitionsResponse(aGetCredentialDefinitionsRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
