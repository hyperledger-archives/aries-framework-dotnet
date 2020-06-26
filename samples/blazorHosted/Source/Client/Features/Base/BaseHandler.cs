namespace BlazorHosted.Features.Bases
{
  using BlazorHosted.Features.Applications;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Features.Counters;
  using BlazorHosted.Features.CredentialDefinitions;
  using BlazorHosted.Features.Credentials;
  using BlazorHosted.Features.PresentProof;
  using BlazorHosted.Features.Schemas;
  using BlazorHosted.Features.Wallets;
  using BlazorHosted.Features.WeatherForecasts;
  using BlazorState;

  /// <summary>
  /// Base Handler that makes it easy to access state
  /// </summary>
  /// <typeparam name="TAction"></typeparam>
  internal abstract class BaseHandler<TAction> : ActionHandler<TAction>
    where TAction : IAction
  {
    protected ApplicationState ApplicationState => Store.GetState<ApplicationState>();
    protected ConnectionState ConnectionState => Store.GetState<ConnectionState>();
    protected CounterState CounterState => Store.GetState<CounterState>();
    protected CredentialDefinitionState CredentialDefinitionState => Store.GetState<CredentialDefinitionState>();
    protected CredentialState CredentialState => Store.GetState<CredentialState>();
    protected PresentProofState PresentProofState => Store.GetState<PresentProofState>();
    protected SchemaState SchemaState => Store.GetState<SchemaState>();
    protected WalletState WalletState => Store.GetState<WalletState>();
    protected WeatherForecastsState WeatherForecastsState => Store.GetState<WeatherForecastsState>();

    public BaseHandler(IStore aStore) : base(aStore) { }
  }
}
