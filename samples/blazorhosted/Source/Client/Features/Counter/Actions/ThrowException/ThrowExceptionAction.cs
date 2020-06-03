namespace blazorhosted.Features.Counters
{
  using blazorhosted.Features.Bases;

  internal partial class CounterState
  {
    public class ThrowExceptionAction : BaseAction
    {
      public string Message { get; set; }
    }
  }
}
