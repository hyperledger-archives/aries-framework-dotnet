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
  using BlazorState.Pipeline.ReduxDevTools;

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
    internal ConnectionState ConnectionState => GetState<ConnectionState>();
    internal CounterState CounterState => GetState<CounterState>();
    internal CredentialDefinitionState CredentialDefinitionState => GetState<CredentialDefinitionState>();
    internal CredentialState CredentialState => GetState<CredentialState>();
    internal PresentProofState PresentProofState => GetState<PresentProofState>();
    internal SchemaState SchemaState => GetState<SchemaState>();
    internal WalletState WalletState => GetState<WalletState>();
  }
}
