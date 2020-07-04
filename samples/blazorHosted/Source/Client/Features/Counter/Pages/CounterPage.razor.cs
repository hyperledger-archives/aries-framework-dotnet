namespace Hyperledger.Aries.OpenApi.Pages
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
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
