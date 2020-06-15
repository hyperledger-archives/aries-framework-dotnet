namespace BlazorHosted.Features.Wallets
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Configuration;

  //TODO  Maybe just inherit from Provision Record?

  /// <summary>
  /// We could just inherit
  /// </summary>
  public class GetWalletResponse : BaseResponse
  {
    public ProvisioningRecord ProvisioningRecord { get; set; } = null!;
    public GetWalletResponse() { }

    public GetWalletResponse(Guid aRequestId, ProvisioningRecord aProvisioningRecord) : base(aRequestId) 
    {
      ProvisioningRecord = aProvisioningRecord;
    }
  }
}
