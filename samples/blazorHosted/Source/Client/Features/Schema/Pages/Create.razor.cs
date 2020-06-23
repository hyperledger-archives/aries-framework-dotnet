namespace BlazorHosted.Features.Schemas.Pages
{
  using BlazorHosted.Features.Bases;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Schemas.SchemaState;

  public partial class Create : BaseComponent
  {
    public const string RouteTemplate = "/schemas/create";
    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick() =>
      _ = await Mediator.Send(new CreateSchemaAction());
  }
}
