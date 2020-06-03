namespace blazorhosted.Components
{
  using blazorhosted.Features.Bases;
  using static blazorhosted.Features.Applications.ApplicationState;

  public partial class ResetButton:BaseComponent
  {
    internal void ButtonClick() => Mediator.Send(new ResetStoreAction());
  }
}
