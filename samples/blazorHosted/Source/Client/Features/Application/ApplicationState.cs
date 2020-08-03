namespace Hyperledger.Aries.AspNetCore.Features.Applications
{
  using BlazorState;

  internal partial class ApplicationState : State<ApplicationState>
  {
    public bool IsMenuExpanded { get; private set; }
    public string Logo { get; private set; }
    public string Name { get; private set; }
    public string Version => GetType().Assembly.GetName().Version.ToString();

    public ApplicationState() { }

    public override void Initialize()
    {
      IsMenuExpanded = true;
      Name = "Hyperledger.Aries.AspNetCore";
      Logo = "/images/logo.png";
    }
  }
}
