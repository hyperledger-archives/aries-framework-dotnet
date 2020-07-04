namespace Hyperledger.Aries.OpenApi.Features.Applications.Components
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public partial class SideBar: BaseComponent
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";
  }
}
