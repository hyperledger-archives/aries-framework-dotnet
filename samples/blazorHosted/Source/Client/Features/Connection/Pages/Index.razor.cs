namespace Hyperledger.Aries.AspNetCore.Features.Connections.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using static Hyperledger.Aries.AspNetCore.Features.Connections.ConnectionState;
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
