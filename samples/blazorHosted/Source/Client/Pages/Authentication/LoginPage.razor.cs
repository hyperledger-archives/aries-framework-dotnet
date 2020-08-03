namespace Hyperledger.Aries.AspNetCore.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class LoginPage : BaseComponent
  {
    public const string RouteTemplate = "Login";

    public static string GetRoute() => RouteTemplate;
  }
}
