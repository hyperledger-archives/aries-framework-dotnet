namespace BlazorHosted.Features.CredentialDefinitions
{
  using FluentValidation;

  public class GetCredentialDefinitionRequestValidator : AbstractValidator<GetCredentialDefinitionRequest>
  {
    public GetCredentialDefinitionRequestValidator()
    {
      RuleFor(aGetCredentialDefinitionRequest => aGetCredentialDefinitionRequest.CredentialDefinitionId)
        .NotEmpty();
    }
  }
}
