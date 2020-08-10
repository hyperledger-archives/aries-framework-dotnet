namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using FluentValidation;

  public class AcceptProofRequestRequestValidator : AbstractValidator<AcceptProofRequestRequest>
  {
    public AcceptProofRequestRequestValidator()
    {
      RuleFor(aAcceptProofRequestRequest => aAcceptProofRequestRequest.EncodedProofRequestMessage)
        .NotEmpty();
    }
  }
}
