namespace BlazorHosted.Features.PresentProofs
{
  using BlazorHosted.Features.Bases;
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class PresentProofState
  {
    internal class CreateProofRequestHandler : BaseHandler<CreateProofRequestAction>
    {
      private readonly HttpClient HttpClient;

      public CreateProofRequestHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        CreateProofRequestAction aCreateAndSendProofRequestAction,
        CancellationToken aCancellationToken
      )
      {
        CreateProofRequestRequest sendRequestForProofRequest = aCreateAndSendProofRequestAction.CreateProofRequestRequest;

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateProofRequestRequest>
          (
            sendRequestForProofRequest.GetRoute(),
            sendRequestForProofRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();

        return Unit.Value;
      }
    }
  }
}
