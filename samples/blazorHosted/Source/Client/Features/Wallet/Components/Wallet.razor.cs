namespace BlazorHosted.Features.Wallets.Components
{
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Wallets.WalletState;
  using BlazorHosted.Features.Bases;

  public partial class Wallet : BaseComponent
  {
 
    protected override async Task OnInitializedAsync() =>
      await Mediator.Send(new FetchWalletAction());
  }
}
