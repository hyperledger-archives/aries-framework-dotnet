namespace BlazorHosted.Features.Wallets
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetWalletHandler : IRequestHandler<GetWalletRequest, GetWalletResponse>
  {

    public async Task<GetWalletResponse> Handle
    (
      GetWalletRequest aGetWalletRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetWalletResponse(aGetWalletRequest.RequestId);

      return await Task.Run(() => response);
    }
  }
}
