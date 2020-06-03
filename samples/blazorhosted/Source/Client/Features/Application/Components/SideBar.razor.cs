namespace blazorhosted.Features.Applications.Components
{
  using blazorhosted.Features.Bases;

  public partial class SideBar: BaseComponent
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";
  }
}
