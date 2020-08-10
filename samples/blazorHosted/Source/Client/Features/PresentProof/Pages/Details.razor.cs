namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs.Pages
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;
  using Microsoft.AspNetCore.Components;
  using Newtonsoft.Json;
  using System.Threading.Tasks;

  public partial class Details : BaseComponent
  {
    public const string RouteTemplate = "/proofs/{ProofId}";

    [Parameter] public string ProofId { get; set; } = null!;

    public ProofRecord ProofRecord => PresentProofState.Proofs[ProofId];

    public ProofRequest ProofRequest => string.IsNullOrEmpty(ProofRecord.RequestJson) ? null :JsonConvert.DeserializeObject<ProofRequest>(ProofRecord.RequestJson);

    public PartialProof PartialProof => string.IsNullOrEmpty(ProofRecord.ProofJson) ? null : JsonConvert.DeserializeObject<PartialProof>(ProofRecord.ProofJson);

    public static string GetRoute(string aProofId) =>
      RouteTemplate
        .Replace($"{{{nameof(ProofId)}}}", aProofId, System.StringComparison.OrdinalIgnoreCase);

    protected override Task OnParametersSetAsync()
    {
      //_ = await Mediator.Send(new FetchMathcingCredentialsAction());
      return base.OnParametersSetAsync();
    }
  }
}
