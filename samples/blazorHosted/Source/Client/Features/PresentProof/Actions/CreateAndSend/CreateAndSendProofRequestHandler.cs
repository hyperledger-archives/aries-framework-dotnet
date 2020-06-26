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
    internal class CreateAndSendProofRequestHandler : BaseHandler<CreateAndSendProofRequestAction>
    {
      private readonly HttpClient HttpClient;

      public CreateAndSendProofRequestHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        CreateAndSendProofRequestAction aCreateAndSendProofRequestAction,
        CancellationToken aCancellationToken
      )
      {
        SendRequestForProofRequest sendRequestForProofRequest = aCreateAndSendProofRequestAction.SendRequestForProofRequest;

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<SendRequestForProofRequest>
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
