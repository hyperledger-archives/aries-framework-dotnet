namespace Hyperledger.Aries.AspNetCore.Features.Counters
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class CounterState
  {
    public class ThrowExceptionAction : BaseAction
    {
      public string Message { get; set; }
    }
  }
}
