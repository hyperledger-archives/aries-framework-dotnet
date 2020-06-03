namespace blazorhosted.Pages
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using blazorhosted.Features.Bases;

  public partial class CounterPage: BaseComponent
  {
    public const string Route = "/counter";

    protected async Task ButtonClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = "/" });
  }
}
