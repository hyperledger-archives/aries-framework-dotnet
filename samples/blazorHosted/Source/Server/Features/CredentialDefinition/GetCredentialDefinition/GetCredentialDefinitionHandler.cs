namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetCredentialDefinitionHandler : IRequestHandler<GetCredentialDefinitionRequest, GetCredentialDefinitionResponse>
  {

    public async Task<GetCredentialDefinitionResponse> Handle
    (
      GetCredentialDefinitionRequest aGetCredentialDefinitionRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetCredentialDefinitionResponse(aGetCredentialDefinitionRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
