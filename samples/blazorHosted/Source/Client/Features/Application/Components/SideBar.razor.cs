namespace BlazorHosted.Features.Applications.Components
{
  using BlazorHosted.Features.Bases;

  public partial class SideBar: BaseComponent
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";
  }
}
