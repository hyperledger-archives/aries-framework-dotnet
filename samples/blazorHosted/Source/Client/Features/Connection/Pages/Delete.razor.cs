namespace BlazorHosted.Features.Connections.Pages
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using Microsoft.AspNetCore.Components;

  public partial class Delete: BaseComponent
  {
    public const string RouteTemplate = "/connections/delete/{ConnectionId}";

    public static string GetRoute(string aConnectionId) =>
      RouteTemplate
        .Replace($"{{{nameof(ConnectionId)}}}", aConnectionId.ToString(), System.StringComparison.OrdinalIgnoreCase);

    [Parameter] public string ConnectionId { get; set; }

    //protected async Task DeleteClick()
    //{
    //  _ = await Mediator.Send(new DeleteCatalogItemAction { CatalogItemId = EntityId });
    //  _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.RouteTemplate });
    //}

    //protected async Task CancelClick() =>
    //  _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.RouteTemplate });
  }
}