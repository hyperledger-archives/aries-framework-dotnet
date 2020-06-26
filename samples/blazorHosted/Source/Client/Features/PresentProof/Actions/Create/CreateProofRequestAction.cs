namespace BlazorHosted.Features.PresentProofs
{
  using BlazorHosted.Features.Bases;

  internal partial class PresentProofState
  {
    public class CreateProofRequestAction : BaseAction
    {
      public CreateProofRequestRequest CreateProofRequestRequest { get; set; }
    }
  }
}
