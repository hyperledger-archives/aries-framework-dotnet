namespace BlazorHosted.Features.Wallets
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class ResetWalletResponse : BaseResponse
  {
    public ResetWalletResponse() { }

    public ResetWalletResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
