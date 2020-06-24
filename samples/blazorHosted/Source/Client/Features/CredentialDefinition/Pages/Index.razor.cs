namespace BlazorHosted.Features.CredentialDefinitions.Pages
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.CredentialDefinitions.CredentialDefinitionState;
  using static BlazorHosted.Features.Schemas.SchemaState;

  public partial class Index
  {
    public const string RouteTemplate = "/credential-definitions";

    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = Create.GetRoute() });

    protected override Task OnInitializedAsync()
    {
      _ = Mediator.Send(new FetchSchemasAction());
      _ = Mediator.Send(new FetchCredentialDefinitionsAction());
      return base.OnInitializedAsync();
    }
  }
}
