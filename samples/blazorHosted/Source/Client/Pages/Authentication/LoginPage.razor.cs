namespace BlazorHosted.Pages
{
  using BlazorHosted.Features.Bases;

  public partial class LoginPage : BaseComponent
  {
    public const string RouteTemplate = "Login";

    public static string GetRoute() => RouteTemplate;
  }
}
