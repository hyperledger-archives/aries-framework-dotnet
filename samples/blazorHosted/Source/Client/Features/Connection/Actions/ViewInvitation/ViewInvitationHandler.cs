namespace BlazorHosted.Features.Connections
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;
  using Newtonsoft.Json;
  using System.Linq;
  using System.Net.Http.Json;
  using Hyperledger.Aries.Utils;
  using Hyperledger.Aries.Features.DidExchange;

  internal partial class ConnectionState
  {
    public class ViewInvitationHandler : BaseHandler<ViewInvitationAction>
    {
      private readonly HttpClient HttpClient;

      public ViewInvitationHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

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
