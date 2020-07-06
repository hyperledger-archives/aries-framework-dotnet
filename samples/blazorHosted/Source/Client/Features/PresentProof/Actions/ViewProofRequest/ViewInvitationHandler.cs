namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using BlazorState;
  using MediatR;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Newtonsoft.Json;
  using System.Linq;
  using System.Net.Http.Json;
  using Hyperledger.Aries.Utils;
  using Hyperledger.Aries.Features.DidExchange;
  using System;
  using Hyperledger.Aries.Features.PresentProof;

  internal partial class PresentProofState
  {
    public class ViewInvitationHandler : BaseHandler<ViewProofRequestAction>
    {
      public ViewInvitationHandler(IStore aStore) : base(aStore) { }

      public override Task<Unit> Handle
      (
        ViewProofRequestAction aViewProofRequestAction,
        CancellationToken aCancellationToken
      )
      {
        PresentProofState.RequestPresentationMessage =
           MessageUtils
            .DecodeMessageFromUrlFormat<RequestPresentationMessage>(aViewProofRequestAction.EncodedProofRequestMessage);

        return Unit.Task;
      }
    }
  }
}
