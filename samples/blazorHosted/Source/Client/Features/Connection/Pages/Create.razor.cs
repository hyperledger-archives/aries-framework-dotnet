namespace BlazorHosted.Pages.Connections
{
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.Connections;
  using System;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Connections.ConnectionState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/Connections/Create";

    public CreateInvitationRequest CreateInvitationRequest { get; set; }

    public static string GetRoute() => RouteTemplate;

    protected async Task ButtonClick()
    {
      _ = await Mediator.Send(new CreateConnectionAction());
    }

    protected override async Task OnInitializedAsync()
    {
      Console.WriteLine("Where are you?");
      _ = await Mediator.Send(new CreateConnectionAction());

      await base.OnInitializedAsync();
    }
  }
}
