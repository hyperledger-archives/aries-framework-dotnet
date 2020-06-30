namespace BlazorHosted.Features.Wallets.Pages
{
  using BlazorHosted.Features.Bases;
  using System.Linq;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Wallets.WalletState;

  public partial class WalletPage : BaseComponent
  {
    public const string RouteTemplate = "/wallet";

    public static string GetRoute() => RouteTemplate;

    public string Did => WalletState?.ProvisioningRecord?.Endpoint?.Did;
    public string VerKey => WalletState?.ProvisioningRecord?.Endpoint?.Verkey?.FirstOrDefault();
    public string Uri => WalletState?.ProvisioningRecord?.Endpoint?.Uri;
    public string Name => WalletState?.ProvisioningRecord?.Owner?.Name;

    protected override async Task OnInitializedAsync() =>
     _ = await Mediator.Send(new FetchWalletAction());

  }
}
