namespace BlazorHosted.Features.Wallets
{
  using BlazorState;
  using System;

  internal partial class WalletState : State<WalletState>
  {
    public string Name { get; private set; }
    public Uri Uri { get; private set; }
    public string Did { get; private set; }
    public string VerKey { get; private set; }

    public WalletState() { }

    /// <summary>
    /// Set the Initial State
    /// </summary>
    public override void Initialize()
    {
      Name = "Naughty Lichterman"; // null;
      Uri = new Uri("http://localhost:5000"); // null;
      Did = "LMx45porrc4LoVk3N8s9i1"; // null;
      VerKey = "BZ3dzMNrbHUPCx4acXwCRTHj59pnHDCve54mxonVYUbJ"; // null;
    }
  }
}
