namespace BlazorHosted.Features.Counters
{
  using BlazorHosted.Features.Bases;

  internal partial class CounterState
  {
    public class IncrementCounterAction : BaseAction
    {
      public int Amount { get; set; }
    }
  }
}
