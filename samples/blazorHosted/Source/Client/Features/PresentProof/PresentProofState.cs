namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using BlazorState;
  using Hyperledger.Aries.Features.PresentProof;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json.Serialization;

  internal partial class PresentProofState : State<PresentProofState>
  {
    [JsonIgnore]
    public IReadOnlyDictionary<string, ProofRecord> Proofs => _ProofRecords;

    public IReadOnlyList<ProofRecord> ProofsAsList => _ProofRecords.Values.ToList();
    private Dictionary<string, ProofRecord> _ProofRecords { get; set; }

    public string ProofRequestUrl { get; private set; }

    public string ProofRequestQrUri { get; private set; }

    public RequestPresentationMessage RequestPresentationMessage { get; private set; }

    public PresentProofState() { }

    public override void Initialize()
    {
      _ProofRecords = new Dictionary<string, ProofRecord>();
      ProofRequestUrl = null;
      ProofRequestQrUri = null;
    }
  }
}
