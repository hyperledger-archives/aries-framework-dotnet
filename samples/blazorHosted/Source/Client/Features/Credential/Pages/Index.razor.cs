namespace BlazorHosted.Features.Credentials.Pages
{
  using BlazorState.Features.Routing;
  using BlazorHosted.Features.Bases;
  using static BlazorHosted.Features.Credentials.CredentialState;
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
