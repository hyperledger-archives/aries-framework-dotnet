namespace blazorhosted.Features.Applications.Components
{
  using static blazorhosted.Features.Applications.ApplicationState;

  public partial class NavBar
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";

    protected async void ToggleNavMenu() => await Mediator.Send(new ToggleMenuAction());
  }
}
