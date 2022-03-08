namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Aries.Features.Handshakes.Connection.Models;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;
  using Utils;

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
