namespace Hyperledger.Aries.OpenApi.Features.Schemas.Pages
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.OpenApi.Features.Schemas.SchemaState;
  using static BlazorState.Features.Routing.RouteState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/schemas/create";
    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick()
    {
      _ = await Mediator.Send(new CreateSchemaAction());
      _ = await Mediator.Send(new ChangeRouteAction { NewRoute = Pages.Index.RouteTemplate });
    }
  }
}
