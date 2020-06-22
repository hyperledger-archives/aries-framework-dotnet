namespace BlazorHosted.Features.Connections.Components
{
  using BlazorHosted.Features.Bases;
  using Microsoft.AspNetCore.Components;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Create: BaseComponent
  {
    public CreateInvitationRequest CreateInvitationRequest { get; set; }

    [Parameter] public string RedirectRoute { get; set; }

    protected override Task OnInitializedAsync()
    {
      CreateInvitationRequest = new CreateInvitationRequest();
      
      return base.OnInitializedAsync();
    }

    protected async Task HandleValidSubmit()
    {
      _ = await Mediator.Send(new CreateConnectionAction { CreateInvitationRequest = CreateInvitationRequest });
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = RedirectRoute });
    }
  }
}
