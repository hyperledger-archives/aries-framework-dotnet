namespace Hyperledger.Aries.AspNetCore.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;

  public partial class CounterPage : BaseComponent
  {
    public const string RouteTemplate = "/counter";

    public static string GetRoute() => RouteTemplate;

    protected async Task ButtonClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = "/" });
  }
}
