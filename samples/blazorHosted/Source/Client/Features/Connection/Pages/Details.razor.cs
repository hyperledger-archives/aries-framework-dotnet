namespace Hyperledger.Aries.AspNetCore.Features.Connections.Pages
{
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Microsoft.AspNetCore.Components;
  using static Hyperledger.Aries.AspNetCore.Features.Connections.ConnectionState;
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

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchConnectionAction(ConnectionId));
      await base.OnInitializedAsync();
    }
  }
}
