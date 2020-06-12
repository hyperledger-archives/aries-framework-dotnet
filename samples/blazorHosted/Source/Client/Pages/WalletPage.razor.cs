namespace BlazorHosted.Pages
{
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using static BlazorHosted.Features.Wallets.WalletState;

  public partial class WalletPage : BaseComponent
  {
    public const string Route = "/wallet";

    protected override async Task OnInitializedAsync() =>
      await Mediator.Send(new FetchWalletAction());
  }
}
