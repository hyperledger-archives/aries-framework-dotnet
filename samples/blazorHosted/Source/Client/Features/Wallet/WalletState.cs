namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using BlazorState;
  using Hyperledger.Aries.Configuration;

  internal partial class WalletState : State<WalletState>
  {
    public bool IsCached { get; private set; }
    public ProvisioningRecord ProvisioningRecord { get; private set; }

    public WalletState() { }

    /// <summary>
    /// Set the Initial State
    /// </summary>
    public override void Initialize()
    {
      IsCached = false;
      ProvisioningRecord = null;
    }
  }
}
