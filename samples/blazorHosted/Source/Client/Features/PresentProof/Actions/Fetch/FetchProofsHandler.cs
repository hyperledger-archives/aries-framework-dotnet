namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using BlazorState;
  using MediatR;
  using Newtonsoft.Json;
  using System.Linq;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading;
  using System.Threading.Tasks;

  internal partial class PresentProofState
  {
    internal class FetchProofsHandler : BaseHandler<FetchProofsAction>
    {
      private readonly HttpClient HttpClient;

      public FetchProofsHandler
      (
        IStore aStore,
        HttpClient aHttpClient
      ) : base(aStore)
      {
        HttpClient = aHttpClient;
      }

      public override async Task<Unit> Handle
      (
        FetchProofsAction aFetchProofsAction,
        CancellationToken aCancellationToken
      )
      {
        var getProofsRequest = new GetProofsRequest();

        HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(getProofsRequest.GetRoute());
        string json = await httpResponseMessage.Content.ReadAsStringAsync();
        GetProofsResponse getProofsResponse = JsonConvert.DeserializeObject<GetProofsResponse>(json);

        PresentProofState._ProofRecords = getProofsResponse.ProofRecords
          .ToDictionary(aProofRecord => aProofRecord.Id, aProofRecord => aProofRecord);

        return Unit.Value;
      }
    }
  }
}
