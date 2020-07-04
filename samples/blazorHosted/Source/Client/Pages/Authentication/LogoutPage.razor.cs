namespace Hyperledger.Aries.OpenApi.Pages
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public partial class LogoutPage : BaseComponent
  {
    public const string RouteTemplate = "Logout";

    public static string GetRoute() => RouteTemplate;
  }
}
