namespace Hyperledger.Aries.AspNetCore.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class LogoutPage : BaseComponent
  {
    public const string RouteTemplate = "Logout";

    public static string GetRoute() => RouteTemplate;
  }
}
