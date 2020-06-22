namespace BlazorHosted.Pages.Connections
{
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.Connections;
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/Connections/Create";

    public CreateInvitationRequest CreateInvitationRequest { get; set; }

    public static string GetRoute() => RouteTemplate;

    protected async Task ButtonClick()
    {
      _ = await Mediator.Send(new CreateConnectionAction { CreateInvitationRequest = CreateInvitationRequest });
    }

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new CreateConnectionAction { CreateInvitationRequest = CreateInvitationRequest });

      await base.OnInitializedAsync();
    }
  }
}
