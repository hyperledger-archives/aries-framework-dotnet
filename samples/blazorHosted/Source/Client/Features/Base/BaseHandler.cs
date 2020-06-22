namespace BlazorHosted.Features.Bases
{
  using BlazorState;
  using BlazorHosted.Features.Applications;
  using BlazorHosted.Features.Counters;
  using BlazorHosted.Features.WeatherForecasts;
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Features.Connections;

  /// <summary>
  /// Base Handler that makes it easy to access state
  /// </summary>
  /// <typeparam name="TAction"></typeparam>
  internal abstract class BaseHandler<TAction> : ActionHandler<TAction>
    where TAction : IAction
  {
    protected ApplicationState ApplicationState => Store.GetState<ApplicationState>();

    protected CounterState CounterState => Store.GetState<CounterState>();

    protected ConnectionState ConnectionState => Store.GetState<ConnectionState>();

    protected WalletState WalletState => Store.GetState<WalletState>();

    protected WeatherForecastsState WeatherForecastsState => Store.GetState<WeatherForecastsState>();

    public BaseHandler(IStore aStore) : base(aStore) { }
  }
}
