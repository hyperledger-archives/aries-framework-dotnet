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
