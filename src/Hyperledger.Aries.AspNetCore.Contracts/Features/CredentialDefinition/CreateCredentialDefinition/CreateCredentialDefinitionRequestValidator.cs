namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using FluentValidation;
  
  public class CreateCredentialDefinitionRequestValidator : AbstractValidator<CreateCredentialDefinitionRequest>
  {

    public CreateCredentialDefinitionRequestValidator()
    {
      RuleFor(aCreateCredentialDefinitionRequest => aCreateCredentialDefinitionRequest.SchemaId)
        .NotEmpty()
        .Matches("^[123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz]{21,22}:2:.+:[0-9.]+$");
    }
  }
}