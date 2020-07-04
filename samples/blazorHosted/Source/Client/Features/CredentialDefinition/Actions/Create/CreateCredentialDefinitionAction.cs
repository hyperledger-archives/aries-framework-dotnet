namespace Hyperledger.Aries.OpenApi.Features.CredentialDefinitions
{
  using Hyperledger.Aries.OpenApi.Features.Bases;

  internal partial class CredentialDefinitionState
  {
    public class CreateCredentialDefinitionAction : BaseAction
    {
      public CreateCredentialDefinitionRequest CreateCredentialDefinitionRequest { get; set; }
    }
  }
}
