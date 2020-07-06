namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class PresentProofState
  {
    public class ViewProofRequestAction : BaseAction 
    {
      public string EncodedProofRequestMessage { get; set; }
    }
  }
}
