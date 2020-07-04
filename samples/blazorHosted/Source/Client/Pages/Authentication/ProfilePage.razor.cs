namespace Hyperledger.Aries.OpenApi.Pages
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public partial class ProfilePage : BaseComponent
  {
    public const string RouteTemplate = "/Profile";

    public static string GetRoute() => RouteTemplate;
  }
}
