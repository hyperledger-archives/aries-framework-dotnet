namespace BlazorHosted.Features.Bases
{
  using BlazorState.Pipeline.ReduxDevTools;
  using BlazorHosted.Features.Applications;
  using BlazorHosted.Features.Counters;
  using BlazorHosted.Features.WeatherForecasts;
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Features.Schemas;
  using BlazorHosted.Features.CredentialDefinitions;

  /// <summary>
  /// Makes access to the State a little easier and by inheriting from
  /// BlazorStateDevToolsComponent it allows for ReduxDevTools operation.
  /// </summary>
  /// <remarks>
  /// In production one would NOT be required to use these base components
  /// But would be required to properly implement the required interfaces.
  /// one could conditionally inherit from BaseComponent for production build.
  /// </remarks>
  public class BaseComponent : BlazorStateDevToolsComponent
  {
    internal ApplicationState ApplicationState => GetState<ApplicationState>();
    internal CounterState CounterState => GetState<CounterState>();
    internal ConnectionState ConnectionState => GetState<ConnectionState>();
    internal CredentialDefinitionState CredentialDefinitionState => GetState<CredentialDefinitionState>();
    internal SchemaState SchemaState => GetState<SchemaState>();
    internal WalletState WalletState => GetState<WalletState>();
    internal WeatherForecastsState WeatherForecastsState => GetState<WeatherForecastsState>();
  }
}
