namespace Hyperledger.Aries.AspNetCore.Features.Applications.Components
{
  using static Hyperledger.Aries.AspNetCore.Features.Applications.ApplicationState;

  public partial class NavBar
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";

    protected async void ToggleNavMenu() => await Mediator.Send(new ToggleMenuAction());
  }
}
