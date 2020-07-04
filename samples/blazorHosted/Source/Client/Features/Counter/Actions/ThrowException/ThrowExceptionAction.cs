namespace Hyperledger.Aries.OpenApi.Features.Counters
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class CounterState
  {
    public class ThrowExceptionAction : BaseAction
    {
      public string Message { get; set; }
    }
  }
}
