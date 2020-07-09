namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Newtonsoft.Json;
  using System.Linq;

  internal partial class CredentialDefinitionState
  {
    public class FetchCredentialDefinitionsHandler : BaseHandler<FetchCredentialDefinitionsAction>
    {
      private readonly HttpClient HttpClient;

      public FetchCredentialDefinitionsHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchCredentialDefinitionsAction aFetchConnectionsAction,
        CancellationToken aCancellationToken
      )
      {
        var getCredentialDefinitionsRequest = new GetCredentialDefinitionsRequest();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(getCredentialDefinitionsRequest.GetRoute());
        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        GetCredentialDefinitionsResponse getCredentialDefinitionsResponse = JsonConvert.DeserializeObject<GetCredentialDefinitionsResponse>(json);

        CredentialDefinitionState._CredentialDefinitionRecords = getCredentialDefinitionsResponse.DefinitionRecords
          .ToDictionary(aCredentialDefinitionRecord => aCredentialDefinitionRecord.Id, aCredentialDefinitionRecord => aCredentialDefinitionRecord);

        return Unit.Value;
      }
    }
  }
}
