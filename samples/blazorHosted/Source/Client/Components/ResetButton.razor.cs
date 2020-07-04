namespace Hyperledger.Aries.OpenApi.Components
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using static Hyperledger.Aries.OpenApi.Features.Applications.ApplicationState;

  public partial class ResetButton:BaseComponent
  {
    internal void ButtonClick() => Mediator.Send(new ResetStoreAction());
  }
}
