namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class CreateInvitationHandler : IRequestHandler<CreateInvitationRequest, CreateInvitationResponse>
  {

    public async Task<CreateInvitationResponse> Handle
    (
      CreateInvitationRequest aCreateInvitationRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new CreateInvitationResponse(aCreateInvitationRequest.Id);

      return await Task.Run(() => response);
    }
  }
}
