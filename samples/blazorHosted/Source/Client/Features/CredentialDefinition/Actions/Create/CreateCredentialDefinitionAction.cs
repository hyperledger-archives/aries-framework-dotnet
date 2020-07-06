namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  internal partial class CredentialDefinitionState
  {
    public class CreateCredentialDefinitionAction : BaseAction
    {
      public CreateCredentialDefinitionRequest CreateCredentialDefinitionRequest { get; set; }
    }
  }
}
