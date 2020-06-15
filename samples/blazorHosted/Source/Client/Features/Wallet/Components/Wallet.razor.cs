namespace BlazorHosted.Features.Wallets.Components
{
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Wallets.WalletState;
  using BlazorHosted.Features.Bases;

  public partial class Wallet : BaseComponent
  {
    public System.Uri IndentIconUrl => new System.Uri($"api/identicon?value={WalletState.Name}&size=60");

    //protected async Task ButtonClick() =>
    //  _ = await Mediator.Send(new IncrementCounterAction { Amount = 5 });
  }
}
