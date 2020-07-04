namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using Hyperledger.Aries.Features.DidExchange;
  using Hyperledger.Aries.Utils;
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
      ConnectionInvitationMessage connectionInvitationMessage = 
        MessageUtils
          .DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(aRecieveInvitationRequest.InvitationDetails);

      var response = 
        new RecieveInvitationResponse(aRecieveInvitationRequest.CorrelationId, connectionInvitationMessage);
      
      return await Task.Run(() => response);
    }
  }
}
