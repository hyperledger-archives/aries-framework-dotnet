namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Utils;
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class ReceiveInvitationHandler : IRequestHandler<ReceiveInvitationRequest, ReceiveInvitationResponse>
  {

    public async Task<ReceiveInvitationResponse> Handle
    (
      ReceiveInvitationRequest aReceiveInvitationRequest,
      CancellationToken aCancellationToken
    )
    {
      ConnectionInvitationMessage connectionInvitationMessage = 
        MessageUtils
          .DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(aReceiveInvitationRequest.InvitationDetails);

      var response = 
        new ReceiveInvitationResponse(aReceiveInvitationRequest.CorrelationId, connectionInvitationMessage);
      
      return await Task.Run(() => response);
    }
  }
}
