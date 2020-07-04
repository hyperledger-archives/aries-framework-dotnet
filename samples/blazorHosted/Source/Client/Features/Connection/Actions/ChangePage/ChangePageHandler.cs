﻿namespace Hyperledger.Aries.OpenApi.Features.Connections
{
  using BlazorState;
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using MediatR;
  using System.Threading;
  using System.Threading.Tasks;
  internal partial class ConnectionState
  {
    public class ChangePageHandler : BaseHandler<ChangePageAction>
    {
      public ChangePageHandler(IStore aStore) : base(aStore) { }
      
      public override Task<Unit> Handle(ChangePageAction aChangePageAction, CancellationToken aCancellationToken)
      {
        ConnectionState.PageIndex = aChangePageAction.PageIndex;
        return Unit.Task;
      }
    }
  }
}
