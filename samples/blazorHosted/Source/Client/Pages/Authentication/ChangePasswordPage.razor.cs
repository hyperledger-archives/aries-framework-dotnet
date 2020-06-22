namespace BlazorHosted.Pages
{
  using BlazorHosted.Features.Bases;

  public partial class ChangePasswordPage : BaseComponent
  {
    public const string RouteTemplate = "/changePassword";
    public static string GetRoute() => RouteTemplate;
  }
}
