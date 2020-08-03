namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using System.Linq;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions.CredentialDefinitionState;
  using static Hyperledger.Aries.AspNetCore.Features.Schemas.SchemaState;
  using static BlazorState.Features.Routing.RouteState;

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
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });
    }

    protected override async Task OnInitializedAsync()
    {
      _ = await Mediator.Send(new FetchSchemasAction());
      await base.OnInitializedAsync();
    }
  }
}
