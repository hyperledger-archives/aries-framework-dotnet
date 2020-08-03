namespace Hyperledger.Aries.AspNetCore.Features.Applications.Components
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class SideBar: BaseComponent
  {
    protected string NavMenuCssClass => ApplicationState.IsMenuExpanded ? null : "collapse";
  }
}
