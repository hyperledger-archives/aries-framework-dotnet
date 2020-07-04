namespace Hyperledger.Aries.OpenApi.Features.Applications.Components
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public partial class Footer: BaseComponent
  {
    protected string Version => ApplicationState.Version;
  }
}
