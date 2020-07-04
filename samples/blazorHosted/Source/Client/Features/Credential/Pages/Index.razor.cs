namespace Hyperledger.Aries.OpenApi.Features.Credentials.Pages
{
  using BlazorState.Features.Routing;
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using static Hyperledger.Aries.OpenApi.Features.Credentials.CredentialState;
  using System.Threading.Tasks;

  public partial class Index : BaseComponent
  {
    public const string RouteTemplate = "/credentials";

    public static string GetRoute() => RouteTemplate;


    protected async Task CreateClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = Edit.GetRoute() });

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchCredentialsAction());
    }
  }
}
