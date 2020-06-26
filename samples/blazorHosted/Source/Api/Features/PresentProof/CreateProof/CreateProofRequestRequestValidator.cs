namespace BlazorHosted.Features.PresentProofs
{
  using FluentValidation;
  
  public class CreateProofRequestRequestValidator : AbstractValidator<CreateProofRequestRequest>
  {

    public CreateProofRequestRequestValidator()
    {
      RuleFor(aCreateProofRequestRequest => aCreateProofRequestRequest.ProofRequest)
        .NotNull();
    }
  }
}