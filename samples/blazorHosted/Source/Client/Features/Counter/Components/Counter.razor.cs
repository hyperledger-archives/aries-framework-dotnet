namespace Hyperledger.Aries.AspNetCore.Features.Counters.Components
{
  using System.Threading.Tasks;
  using static Hyperledger.Aries.AspNetCore.Features.Counters.CounterState;

  public partial class Counter
  {
    protected async Task ButtonClick() =>
      _ = await Mediator.Send(new IncrementCounterAction { Amount = 5 });
  }
}
