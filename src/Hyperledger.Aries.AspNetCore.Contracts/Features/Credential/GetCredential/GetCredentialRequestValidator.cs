namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using FluentValidation;

  public class GetCredentialRequestValidator : AbstractValidator<GetCredentialRequest>
  {
    public GetCredentialRequestValidator()
    {
      RuleFor(aGetCredentialRequest => aGetCredentialRequest.CredentialId)
        .NotEmpty();
    }
  }
}
