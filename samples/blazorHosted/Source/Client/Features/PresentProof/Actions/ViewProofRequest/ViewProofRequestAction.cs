namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class PresentProofState
  {
    public class ViewProofRequestAction : BaseAction 
    {
      public string EncodedProofRequestMessage { get; set; }
    }
  }
}
