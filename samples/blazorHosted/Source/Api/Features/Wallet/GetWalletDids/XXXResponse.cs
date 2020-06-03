namespace BlazorHosted.Features.Wallets
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetWalletDidsResponse : BaseResponse
  {
    public List<DidDto> Dids { get; set; }

    /// <summary>
    /// a default constructor is required for deserialization
    /// </summary>
    public GetWalletDidsResponse() { }

    public GetWalletDidsResponse(Guid aRequestId)
    {
      Dids = new List<DidDto>();
      RequestId = aRequestId;
    }
  }
}
