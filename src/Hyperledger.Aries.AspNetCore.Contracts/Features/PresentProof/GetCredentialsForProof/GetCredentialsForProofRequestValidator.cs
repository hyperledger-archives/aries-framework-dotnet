namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using FluentValidation;

  public class GetCredentialsForProofRequestValidator : AbstractValidator<GetCredentialsForProofRequest>
  {
    public GetCredentialsForProofRequestValidator()
    {
      RuleFor(aGetCredentialsForProofRequest => aGetCredentialsForProofRequest.ProofId)
        .NotEmpty();
    }
  }
}
