namespace BlazorHosted.Components
{
  using BlazorHosted.Features.Bases;
  using static BlazorHosted.Features.Applications.ApplicationState;

  public partial class ResetButton:BaseComponent
  {
    internal void ButtonClick() => Mediator.Send(new ResetStoreAction());
  }
}
