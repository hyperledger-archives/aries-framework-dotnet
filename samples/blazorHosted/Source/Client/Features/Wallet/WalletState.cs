namespace BlazorHosted.Features.Wallets
{
  using BlazorState;
  using Hyperledger.Aries.Configuration;
  using System;

  internal partial class WalletState : State<WalletState>
  {
    public ProvisioningRecord ProvisioningRecord { get; private set; }
    public bool IsCached { get; private set; }

    public WalletState() { }

    /// <summary>
    /// Set the Initial State
    /// </summary>
    public override void Initialize()
    {
      ProvisioningRecord = new ProvisioningRecord();
    }
  }
}
