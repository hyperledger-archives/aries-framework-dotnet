namespace Hyperledger.Aries.AspNetCore.Features.Bases
{
  using Hyperledger.Aries.AspNetCore.Features.Applications;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Features.Counters;
  using Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions;
  using Hyperledger.Aries.AspNetCore.Features.Credentials;
  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;
  using Hyperledger.Aries.AspNetCore.Features.Schemas;
  using Hyperledger.Aries.AspNetCore.Features.Wallets;
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
