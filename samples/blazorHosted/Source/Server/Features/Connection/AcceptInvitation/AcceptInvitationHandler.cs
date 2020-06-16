namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class AcceptInvitationHandler : IRequestHandler<AcceptInvitationRequest, AcceptInvitationResponse>
  {

    public async Task<AcceptInvitationResponse> Handle
    (
      AcceptInvitationRequest aAcceptInvitationRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new AcceptInvitationResponse(aAcceptInvitationRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
