namespace Hyperledger.Aries.AspNetCore.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class ChangePasswordPage : BaseComponent
  {
    public const string RouteTemplate = "/changePassword";
    public static string GetRoute() => RouteTemplate;
  }
}
