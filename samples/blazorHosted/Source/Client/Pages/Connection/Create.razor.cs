namespace BlazorHosted.Pages.Connections
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public partial class Create: BaseComponent
  {
    public const string Route = "/Connections/Create";

    protected async Task ButtonClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = Route });
  }
}
