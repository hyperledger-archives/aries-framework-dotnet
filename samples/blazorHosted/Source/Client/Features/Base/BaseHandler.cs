namespace Hyperledger.Aries.OpenApi.Features.Bases
{
  using Hyperledger.Aries.OpenApi.Features.Applications;
  using Hyperledger.Aries.OpenApi.Features.Connections;
  using Hyperledger.Aries.OpenApi.Features.Counters;
  using Hyperledger.Aries.OpenApi.Features.CredentialDefinitions;
  using Hyperledger.Aries.OpenApi.Features.Credentials;
  using Hyperledger.Aries.OpenApi.Features.PresentProofs;
  using Hyperledger.Aries.OpenApi.Features.Schemas;
  using Hyperledger.Aries.OpenApi.Features.Wallets;
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

    public BaseHandler(IStore aStore) : base(aStore) { }
  }
}
