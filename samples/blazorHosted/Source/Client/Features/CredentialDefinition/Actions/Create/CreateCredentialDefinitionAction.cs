namespace BlazorHosted.Features.CredentialDefinitions
{
  using BlazorHosted.Features.Bases;

  internal partial class CredentialDefinitionState
  {
    public class CreateCredentialDefinitionAction : BaseAction
    {
      public CreateCredentialDefinitionRequest CreateCredentialDefinitionRequest { get; set; }
    }
  }
}
