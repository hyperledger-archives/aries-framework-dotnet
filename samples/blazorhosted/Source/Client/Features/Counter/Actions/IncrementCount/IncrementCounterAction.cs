namespace blazorhosted.Features.Counters
{
  using blazorhosted.Features.Bases;

  internal partial class CounterState
  {
    public class IncrementCounterAction : BaseAction
    {
      public int Amount { get; set; }
    }
  }
}
