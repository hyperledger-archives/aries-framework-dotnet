namespace BlazorHosted.Features.Applications.Components
{
  using BlazorHosted.Features.Bases;

  public partial class Footer: BaseComponent
  {
    protected string Version => ApplicationState.Version;
  }
}
