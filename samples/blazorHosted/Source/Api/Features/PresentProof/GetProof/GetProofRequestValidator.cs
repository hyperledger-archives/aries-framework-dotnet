namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using FluentValidation;

  public class GetProofRequestValidator : AbstractValidator<GetProofRequest>
  {
    public GetProofRequestValidator()
    {
      RuleFor(aGetProofRequest => aGetProofRequest.ProofId)
        .NotEmpty();
    }
  }
}
