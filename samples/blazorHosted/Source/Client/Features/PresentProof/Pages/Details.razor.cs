namespace BlazorHosted.Features.PresentProofs.Pages
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.PresentProof;
  using Microsoft.AspNetCore.Components;
  using Newtonsoft.Json;

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
  }
}
