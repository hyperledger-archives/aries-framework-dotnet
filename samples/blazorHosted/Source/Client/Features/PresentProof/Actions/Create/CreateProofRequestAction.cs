namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class PresentProofState
  {
    public class CreateProofRequestAction : BaseAction
    {
      public CreateProofRequestRequest CreateProofRequestRequest { get; set; }
    }
  }
}
