namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Newtonsoft.Json;
  using System.Linq;
  using System.Net.Http.Json;
  using Hyperledger.Aries.Utils;
  using Hyperledger.Aries.Features.DidExchange;
  using System;

  internal partial class ConnectionState
  {
    public class ViewInvitationHandler : BaseHandler<ViewInvitationAction>
    {
      public ViewInvitationHandler(IStore aStore) : base(aStore) { }

      public override Task<Unit> Handle
      (
        ViewInvitationAction aViewInvitationAction,
        CancellationToken aCancellationToken
      )
      {
        ConnectionState.ConnectionInvitationMessage =
           MessageUtils
            .DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(aViewInvitationAction.InvitationDetails);

        return Unit.Task;
      }
    }
  }
}
