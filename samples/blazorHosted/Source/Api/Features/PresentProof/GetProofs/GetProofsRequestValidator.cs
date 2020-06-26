namespace BlazorHosted.Features.PresentProofs
{
  using FluentValidation;
  
  public class GetProofsRequestValidator : AbstractValidator<GetProofsRequest>
  {

    public GetProofsRequestValidator()
    {
      RuleFor(aGetProofsRequest => aGetProofsRequest.SampleProperty)
        .NotEmpty();
    }
  }
}