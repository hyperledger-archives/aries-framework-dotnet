namespace Hyperledger.Aries.AspNetCore.Features.Schemas.Pages
{
  using BlazorState.Features.Routing;
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.Schemas.SchemaState;

  public partial class Index
  {
    public const string RouteTemplate = "/schemas";

    public static string GetRoute() => RouteTemplate;


    protected async Task CreateClick() =>
      _ = await Mediator.Send(new RouteState.ChangeRouteAction { NewRoute = Create.GetRoute() });

    protected override Task OnInitializedAsync()
    {
      _ = Mediator.Send(new FetchSchemasAction());
      return base.OnInitializedAsync();
    }
  }
}
