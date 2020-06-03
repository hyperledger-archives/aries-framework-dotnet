namespace blazorhosted.Features.Counters.Components
{
  using System.Threading.Tasks;
  using static blazorhosted.Features.Counters.CounterState;

  public partial class Counter
  {
    protected async Task ButtonClick() =>
      _ = await Mediator.Send(new IncrementCounterAction { Amount = 5 });
  }
}
