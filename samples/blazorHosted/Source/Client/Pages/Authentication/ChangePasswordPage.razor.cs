namespace Hyperledger.Aries.OpenApi.Pages
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public partial class ChangePasswordPage : BaseComponent
  {
    public const string RouteTemplate = "/changePassword";
    public static string GetRoute() => RouteTemplate;
  }
}
