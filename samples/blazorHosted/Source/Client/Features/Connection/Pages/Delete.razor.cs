namespace Hyperledger.Aries.OpenApi.Features.Connections.Pages
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Microsoft.AspNetCore.Components;
  using static Hyperledger.Aries.OpenApi.Features.Connections.ConnectionState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Delete: BaseComponent
  {
    public const string RouteTemplate = "/connections/delete/{ConnectionId}";

    public static string GetRoute(string aConnectionId) =>
      RouteTemplate
        .Replace($"{{{nameof(ConnectionId)}}}", aConnectionId.ToString(), System.StringComparison.OrdinalIgnoreCase);

    [Parameter] public string ConnectionId { get; set; }

    protected async Task DeleteClick()
    {
      _ = await Mediator.Send(new DeleteConnectionAction(ConnectionId));
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.RouteTemplate });
    }

    protected async Task CancelClick() =>
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.RouteTemplate });
  }
}
