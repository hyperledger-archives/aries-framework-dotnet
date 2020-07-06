namespace Hyperledger.Aries.AspNetCore.Features.Applications.Components
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public partial class Footer: BaseComponent
  {
    protected string Version => ApplicationState.Version;
  }
}
