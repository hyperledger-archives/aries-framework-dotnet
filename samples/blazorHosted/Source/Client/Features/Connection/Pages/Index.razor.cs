namespace BlazorHosted.Features.Connections.Pages
{
  using BlazorHosted.Features.Bases;
  using static BlazorHosted.Features.Connections.ConnectionState;
  using System.Threading.Tasks;

  public partial class Index: BaseComponent
  {
    public const string RouteTemplate = "/connections";

    public static string GetRoute() => RouteTemplate;

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchConnectionsAction());
      await base.OnInitializedAsync();
    }     
  }
}
