namespace BlazorHosted.Features.PresentProof
{
  using BlazorState;
  using Hyperledger.Aries.Features.PresentProof;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json.Serialization;

  public partial class PresentProofState : State<PresentProofState>
  {
    [JsonIgnore]
    public IReadOnlyDictionary<string, ProofRecord> Proofs => _ProofRecords;

    public IReadOnlyList<ProofRecord> ProofsAsList => _ProofRecords.Values.ToList();
    private Dictionary<string, ProofRecord> _ProofRecords { get; set; }

    public PresentProofState() { }

    public override void Initialize()
    {
      _ProofRecords = new Dictionary<string, ProofRecord>();
    }
  }
}
