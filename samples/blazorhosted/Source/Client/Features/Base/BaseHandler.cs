namespace blazorhosted.Features.Bases
{
  using BlazorState;
  using blazorhosted.Features.Applications;
  using blazorhosted.Features.Counters;
  using blazorhosted.Features.EventStreams;
  using blazorhosted.Features.WeatherForecasts;

  /// <summary>
  /// Base Handler that makes it easy to access state
  /// </summary>
  /// <typeparam name="TAction"></typeparam>
  internal abstract class BaseHandler<TAction> : ActionHandler<TAction>
    where TAction : IAction
  {
    protected ApplicationState ApplicationState => Store.GetState<ApplicationState>();

    protected CounterState CounterState => Store.GetState<CounterState>();

    protected EventStreamState EventStreamState => Store.GetState<EventStreamState>();

    protected WeatherForecastsState WeatherForecastsState => Store.GetState<WeatherForecastsState>();

    public BaseHandler(IStore aStore) : base(aStore) { }
  }
}
