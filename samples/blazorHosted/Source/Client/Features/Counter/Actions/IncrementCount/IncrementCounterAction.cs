namespace Hyperledger.Aries.OpenApi.Features.Counters
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class CounterState
  {
    public class IncrementCounterAction : BaseAction
    {
      public int Amount { get; set; }
    }
  }
}
