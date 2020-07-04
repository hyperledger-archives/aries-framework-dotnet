namespace Hyperledger.Aries.OpenApi.Pages
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public partial class LoginPage : BaseComponent
  {
    public const string RouteTemplate = "Login";

    public static string GetRoute() => RouteTemplate;
  }
}
