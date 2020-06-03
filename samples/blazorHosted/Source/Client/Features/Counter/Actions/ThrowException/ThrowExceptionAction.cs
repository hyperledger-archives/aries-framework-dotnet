namespace BlazorHosted.Features.Counters
{
  using BlazorHosted.Features.Bases;

  internal partial class CounterState
  {
    public class ThrowExceptionAction : BaseAction
    {
      public string Message { get; set; }
    }
  }
}
