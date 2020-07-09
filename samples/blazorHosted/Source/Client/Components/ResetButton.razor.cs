namespace Hyperledger.Aries.AspNetCore.Components
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using static Hyperledger.Aries.AspNetCore.Features.Applications.ApplicationState;

  public partial class ResetButton:BaseComponent
  {
    internal void ButtonClick() => Mediator.Send(new ResetStoreAction());
  }
}
