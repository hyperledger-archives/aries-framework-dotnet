namespace BlazorHosted.Pages
{
  using BlazorHosted.Features.Bases;

  public partial class ProfilePage : BaseComponent
  {
    public const string RouteTemplate = "/Profile";

    public static string GetRoute() => RouteTemplate;
  }
}
