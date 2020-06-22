namespace BlazorHosted.Features.Connections.Pages
{
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using Microsoft.AspNetCore.Components;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Details : BaseComponent
  {
    public const string RouteTemplate = "/connections/details/{ConnectionId}";

    public static string GetRoute(string aConnectionId) =>
      RouteTemplate
        .Replace($"{{{nameof(ConnectionId)}}}", aConnectionId.ToString(), System.StringComparison.OrdinalIgnoreCase);

    [Parameter] public string ConnectionId { get; set; }

    protected async Task BackClick() =>
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.GetRoute() });
  }
}
