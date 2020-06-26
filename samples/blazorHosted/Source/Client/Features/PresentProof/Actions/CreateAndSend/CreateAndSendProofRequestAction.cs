namespace BlazorHosted.Features.PresentProof.Actions.CreateAndSend
{
  using BlazorHosted.Features.PresentProofs;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  internal partial class PresentProofState
  {
    public class CreateAndSendProofRequestAction
    {
      public SendRequestForProofRequest SendRequestForProofRequest { get; set; }
    }
  }
}
