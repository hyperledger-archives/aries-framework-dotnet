namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
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
