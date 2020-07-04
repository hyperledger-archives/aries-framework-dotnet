namespace Hyperledger.Aries.OpenApi.Features.Wallets
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class ResetWalletResponse : BaseResponse
  {
    public ResetWalletResponse() { }

    public ResetWalletResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
