namespace BlazorHosted.Features.PresentProofs
{
  using BlazorHosted.Features.Bases;

  internal partial class PresentProofState
  {
    public class ViewProofRequestAction : BaseAction 
    {
      public string EncodedProofRequestMessage { get; set; }
    }
  }
}
