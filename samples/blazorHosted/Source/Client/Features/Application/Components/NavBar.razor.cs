namespace Hyperledger.Aries.OpenApi.Features.Applications.Components
{
  using static Hyperledger.Aries.OpenApi.Features.Applications.ApplicationState;

  public partial class NavBar
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";

    protected async void ToggleNavMenu() => await Mediator.Send(new ToggleMenuAction());
  }
}
