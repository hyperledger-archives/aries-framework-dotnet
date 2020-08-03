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

  internal partial class ConnectionState
  {
    public class AcceptInvitationHandler : BaseHandler<AcceptInvitationAction>
    {
      private readonly HttpClient HttpClient;

      public AcceptInvitationHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        AcceptInvitationAction aAcceptInvitationAction,
        CancellationToken aCancellationToken
      )
      {
        var acceptInvitationRequest =
          new AcceptInvitationRequest { InvitationDetails = aAcceptInvitationAction.InvitationDetails };

        HttpResponseMessage httpResponseMessage =
          await HttpClient
            .PostAsJsonAsync<AcceptInvitationRequest>(acceptInvitationRequest.GetRoute(), acceptInvitationRequest);

        httpResponseMessage.EnsureSuccessStatusCode();

        return Unit.Value;
      }
    }
  }
}
