namespace BlazorHosted.Features.PresentProofs
{
  using FluentValidation;
  
  public class SendRequestForProofRequestValidator : AbstractValidator<SendRequestForProofRequest>
  {

    public SendRequestForProofRequestValidator()
    {
      RuleFor(aSendRequestForProofRequest => aSendRequestForProofRequest.ProofRequest)
        .NotNull();
    }
  }
}