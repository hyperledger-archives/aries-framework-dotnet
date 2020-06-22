namespace BlazorHosted.Features.Wallets
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.WeatherForecasts;
  using System.Linq;
  using System;
  using Newtonsoft.Json;

  internal partial class WalletState
  {
    public class FetchWallteHandler : BaseHandler<FetchWalletAction>
    {
      private readonly HttpClient HttpClient;

      public FetchWallteHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchWalletAction aFetchWalletAction,
        CancellationToken aCancellationToken
      )
      {
        var getWalletRequest = new GetWalletRequest();
        HttpResponseMessage x = await HttpClient.GetAsync(getWalletRequest.GetRoute());
        string json = await x.Content.ReadAsStringAsync();
        GetWalletResponse getWalletResponse = JsonConvert.DeserializeObject<GetWalletResponse>(json);
        WalletState.Did = getWalletResponse.ProvisioningRecord.Endpoint.Did;
        WalletState.VerKey = getWalletResponse.ProvisioningRecord.Endpoint.Verkey.First();
        WalletState.Uri = getWalletResponse.ProvisioningRecord.Endpoint.Uri;
        WalletState.Name = getWalletResponse.ProvisioningRecord.Owner.Name;

        return Unit.Value;
      }
    }
  }
}
