namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
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

  internal partial class PresentProofState
  {
    public class AcceptProofRequestHandler : BaseHandler<AcceptProofRequestAction>
    {
      private readonly HttpClient HttpClient;

      public AcceptProofRequestHandler(IStore aStore, HttpClient aHttpClient) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        AcceptProofRequestAction aAcceptProofRequestAction,
        CancellationToken aCancellationToken
      )
      {
        var acceptInvitationRequest =
          new AcceptProofRequestRequest { EncodedProofRequestMessage = aAcceptProofRequestAction.EncodedProofRequestMessage };

        HttpResponseMessage httpResponseMessage =
          await HttpClient
            .PostAsJsonAsync<AcceptProofRequestRequest>(acceptInvitationRequest.GetRoute(), acceptInvitationRequest);

        httpResponseMessage.EnsureSuccessStatusCode();

        return Unit.Value;
      }
    }
  }
}
