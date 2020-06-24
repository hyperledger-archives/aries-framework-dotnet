namespace BlazorHosted.Features.Credentials.Pages
{
  using BlazorHosted.Features.Bases;
  using System.Threading.Tasks;
  using static BlazorHosted.Features.Credentials.CredentialState;

  public partial class Edit : BaseComponent
  {
    public const string RouteTemplate = "/credentials/offer";

    public static string GetRoute() => RouteTemplate;

    protected async Task CreateClick() =>
      _ = await Mediator.Send(new OfferCredentialAction());
  }
}
