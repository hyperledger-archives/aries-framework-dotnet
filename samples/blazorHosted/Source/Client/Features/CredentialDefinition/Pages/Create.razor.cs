namespace BlazorHosted.Features.CredentialDefinitions.Pages
{
  using BlazorHosted.Features.Bases;
  using System.Linq;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.CredentialDefinitions.CredentialDefinitionState;
  using static BlazorHosted.Features.Schemas.SchemaState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/credential-definitions/create";

    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick()
    {
      var createCredentialDefinitionAction = new CreateCredentialDefinitionAction()
      {
        CreateCredentialDefinitionRequest = new CreateCredentialDefinitionRequest
        {
          EnableRevocation = false,
          SchemaId = SchemaState.SchemasAsList.First(s => s.Name == "degree-schema").Id,
          Tag = "default",
          RevocationRegistrySize = 0,
          RevocationRegistryBaseUri = null,
          RevocationRegistryAutoScale = false,
        }
      };
      _ = await Mediator.Send(createCredentialDefinitionAction);
    }

    protected override Task OnInitializedAsync()
    {
      _ = Mediator.Send(new FetchSchemasAction());
      return base.OnInitializedAsync();
    }
  }
}
