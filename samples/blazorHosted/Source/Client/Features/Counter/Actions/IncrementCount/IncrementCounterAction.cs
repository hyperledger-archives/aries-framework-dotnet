namespace Hyperledger.Aries.AspNetCore.Features.Counters
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class CounterState
  {
    public class IncrementCounterAction : BaseAction
    {
      public int Amount { get; set; }
    }
  }
}
