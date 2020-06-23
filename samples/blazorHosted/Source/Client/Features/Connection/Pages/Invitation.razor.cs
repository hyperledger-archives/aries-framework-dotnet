namespace BlazorHosted.Features.Connections.Pages
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Utils;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Invitation : BaseComponent
  {
    public const string RouteTemplate = "/connections/invitation";
    public static string GetRoute() => RouteTemplate;

    public string InvitationDetails { get; set; }

    public ConnectionInvitationMessage ConnectionInvitationMessage { get; set; }

    protected async Task OnViewClickAsync() => 
      _ = await Mediator.Send(new ViewInvitationAction { InvitationDetails = InvitationDetails });

    protected async Task OnAcceptClickAsync()
    {
      _ = await Mediator.Send(new AcceptInvitationAction { InvitationDetails = InvitationDetails });
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.GetRoute()});
    }

  }
}
