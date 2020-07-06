namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs.Pages
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.PresentProofs.PresentProofState;

  public partial class Index
  {
    public const string RouteTemplate = "/proofs";

    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = Create.GetRoute() });

    protected async Task ViewClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = View.GetRoute() });

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchProofsAction());

      await base.OnInitializedAsync();
    }
  }
}
