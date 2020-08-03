namespace Hyperledger.Aries.AspNetCore.Features.Applications
{
  using BlazorState;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class ApplicationState
  {
    internal class ToggleMenuHandler : BaseHandler<ToggleMenuAction>
    {
      public ToggleMenuHandler(IStore aStore) : base(aStore) { }

      public override Task<Unit> Handle(ToggleMenuAction aResetStoreAction, CancellationToken aCancellationToken)
      {
        ApplicationState.IsMenuExpanded = !ApplicationState.IsMenuExpanded;
        return Unit.Task;
      }
    }
  }
}
