namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using System;
  using System.Collections.Generic;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class ResetWalletResponse : BaseResponse
  {
    public ResetWalletResponse() { }

    public ResetWalletResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
