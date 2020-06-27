namespace BlazorHosted.Features.PresentProofs
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
