namespace BlazorHosted.Features.CredentialDefinitions
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class CreateCredentialDefinitionHandler : IRequestHandler<CreateCredentialDefinitionRequest, CreateCredentialDefinitionResponse>
  {

    public async Task<CreateCredentialDefinitionResponse> Handle
    (
      CreateCredentialDefinitionRequest aCreateCredentialDefinitionRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new CreateCredentialDefinitionResponse(aCreateCredentialDefinitionRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
