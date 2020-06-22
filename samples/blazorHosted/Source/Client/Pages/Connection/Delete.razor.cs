namespace BlazorHosted.Pages.Connections
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using Microsoft.AspNetCore.Components;

  public partial class Delete: BaseComponent
  {
    public const string Route = "/Connection/Delete/{ConnectionId}";

    public static string RouteFactory(string aConnectionId) =>
      Route.Replace($"{{{nameof(ConnectionId)}}}", aConnectionId.ToString(), System.StringComparison.OrdinalIgnoreCase);

    [Parameter] public string ConnectionId { get; set; }

    protected async Task ButtonClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = "/" });
  }
}
