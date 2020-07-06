namespace Hyperledger.Aries.AspNetCore.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class ProfilePage : BaseComponent
  {
    public const string RouteTemplate = "/Profile";

    public static string GetRoute() => RouteTemplate;
  }
}
