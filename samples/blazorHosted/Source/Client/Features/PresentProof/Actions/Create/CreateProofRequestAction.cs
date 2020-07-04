namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class PresentProofState
  {
    public class CreateProofRequestAction : BaseAction
    {
      public CreateProofRequestRequest CreateProofRequestRequest { get; set; }
    }
  }
}
