namespace BlazorHosted.Pages
{
  using BlazorHosted.Features.Bases;

  public partial class LogoutPage : BaseComponent
  {
    public const string RouteTemplate = "Logout";

    public static string GetRoute() => RouteTemplate;
  }
}
