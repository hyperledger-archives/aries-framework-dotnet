namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class RecieveInvitationHandler : IRequestHandler<RecieveInvitationRequest, RecieveInvitationResponse>
  {

    public async Task<RecieveInvitationResponse> Handle
    (
      RecieveInvitationRequest aRecieveInvitationRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new RecieveInvitationResponse(aRecieveInvitationRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
