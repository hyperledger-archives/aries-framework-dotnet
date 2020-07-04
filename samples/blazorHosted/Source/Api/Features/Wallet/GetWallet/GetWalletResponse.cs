namespace Hyperledger.Aries.OpenApi.Features.Wallets
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.Configuration;
  using System;

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
