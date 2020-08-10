namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class WalletState
  {
    public class ResetWallteHandler : BaseHandler<ResetWalletAction>
    {
      private readonly HttpClient HttpClient;

      public ResetWallteHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        ResetWalletAction aFetchWalletAction,
        CancellationToken aCancellationToken
      )
      {
        var resetWalletRequest = new ResetWalletRequest();

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<ResetWalletRequest>(resetWalletRequest.GetRoute(), resetWalletRequest);

        httpResponseMessage.EnsureSuccessStatusCode();       

        WalletState.ProvisioningRecord = null;
        WalletState.IsCached = false;

        return Unit.Value;
      }
    }
  }
}
