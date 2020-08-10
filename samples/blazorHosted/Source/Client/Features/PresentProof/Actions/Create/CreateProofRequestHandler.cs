namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState;
  using MediatR;
  using Newtonsoft.Json;
  using System;
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
        CreateProofRequestAction aCreateProofRequestAction,
        CancellationToken aCancellationToken
      )
      {
        CreateProofRequestRequest createProofRequestRequest = aCreateProofRequestAction.CreateProofRequestRequest;

        HttpResponseMessage httpResponseMessage =
          await HttpClient.PostAsJsonAsync<CreateProofRequestRequest>
          (
            createProofRequestRequest.GetRoute(),
            createProofRequestRequest
          );

        httpResponseMessage.EnsureSuccessStatusCode();
        //string json = await httpResponseMessage.Content.ReadAsStringAsync();

        //CreateProofRequestResponse createProofRequestResponse =
        //  JsonConvert.DeserializeObject<CreateProofRequestResponse>(json);

        //PresentProofState.RequestPresentationMessage = createProofRequestResponse.RequestPresentationMessage;

        //PresentProofState.ProofRequestUrl = createProofRequestResponse.ProofRequestUrl;
        
        //PresentProofState.ProofRequestQrUri =
        //  $"https://chart.googleapis.com/chart?cht=qr&chs=300x300&chld=L|0&chl=" +
        //  $"{Uri.EscapeDataString(createProofRequestResponse.ProofRequestUrl)}";
  
        return Unit.Value;
      }
    }
  }
}
