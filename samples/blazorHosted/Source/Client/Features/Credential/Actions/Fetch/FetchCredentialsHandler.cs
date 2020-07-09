namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using System.Linq;
  using Newtonsoft.Json;

  internal partial class CredentialState
  {
    public class FetchCredentialsHandler : BaseHandler<FetchCredentialsAction>
    {
      private readonly HttpClient HttpClient;

      public FetchCredentialsHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchCredentialsAction aFetchCredentialsAction,
        CancellationToken aCancellationToken
      )
      {
        var getCredentialsRequest = new GetCredentialsRequest();
        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(getCredentialsRequest.GetRoute());
        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        GetCredentialsResponse getCredentialsResponse = JsonConvert.DeserializeObject<GetCredentialsResponse>(json);

        CredentialState._CredentialRecords = getCredentialsResponse.CredentialRecords
          .ToDictionary(aCredentialRecords => aCredentialRecords.Id, aCredentialRecords => aCredentialRecords);

        return Unit.Value;
      }
    }
  }
}
