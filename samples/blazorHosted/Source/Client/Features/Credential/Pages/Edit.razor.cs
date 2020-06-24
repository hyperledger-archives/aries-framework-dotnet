namespace BlazorHosted.Features.Credentials.Pages
{
  using BlazorHosted.Features.Bases;
  using System.Threading.Tasks;

  public partial class Edit : BaseComponent
  {
    public const string RouteTemplate = "/schemas/create";

    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick() =>
      _ = await Mediator.Send(new OfferCredentialAction());
  }
}
