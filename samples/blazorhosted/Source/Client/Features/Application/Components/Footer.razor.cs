namespace blazorhosted.Features.Applications.Components
{
  using blazorhosted.Features.Bases;

  public partial class Footer: BaseComponent
  {
    protected string Version => ApplicationState.Version;
  }
}
