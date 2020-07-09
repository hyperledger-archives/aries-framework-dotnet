namespace Hyperledger.Aries.AspNetCore.Features.Connections.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Utils;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.Connections.ConnectionState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Invitation : BaseComponent
  {
    public const string RouteTemplate = "/connections/invitation";
    public static string GetRoute() => RouteTemplate;

    public string InvitationDetails { get; set; }

    public ConnectionInvitationMessage ConnectionInvitationMessage => ConnectionState.ConnectionInvitationMessage;

    protected async Task OnViewClickAsync() => _ = await Mediator.Send(new ViewInvitationAction { InvitationDetails = InvitationDetails });

    protected async Task OnAcceptClickAsync()
    {
      _ = await Mediator.Send(new AcceptInvitationAction { InvitationDetails = InvitationDetails });
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Index.GetRoute()});
    }

  }
}
