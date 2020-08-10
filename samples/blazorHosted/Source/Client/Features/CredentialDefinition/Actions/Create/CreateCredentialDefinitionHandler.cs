namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState;
  using MediatR;
  using Newtonsoft.Json;
  using System;
  using System.Collections.Generic;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class CredentialDefinitionState
  {
    public class CreateCredentialDefinitionHandler : BaseHandler<CreateCredentialDefinitionAction>
    {
      private readonly HttpClient HttpClient;

      public CreateCredentialDefinitionHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        CreateCredentialDefinitionAction aCreateCredentialDefinitionAction,
        CancellationToken aCancellationToken
      )
      {
        CreateCredentialDefinitionRequest createInvitationRequest = 
          aCreateCredentialDefinitionAction.CreateCredentialDefinitionRequest;

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateCredentialDefinitionRequest>
          (
            createInvitationRequest.GetRoute(),
            createInvitationRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();
        //string json = await httpResponseMessage.Content.ReadAsStringAsync();

        //CreateCredentialDefinitionResponse createCredentialDefinitionResponse =
        //  JsonConvert.DeserializeObject<CreateCredentialDefinitionResponse>(json);

        return Unit.Value;
      }
    }
  }
}
