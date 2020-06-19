namespace BlazorHosted.Pages
{
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using static BlazorHosted.Features.Connections.ConnectionState;

  public partial class ConnectionsPage : BaseComponent
  {
    public const string Route = "/connections";

    protected override async Task OnInitializedAsync() =>
      await Mediator.Send(new FetchConnectionsAction());
  }
}
