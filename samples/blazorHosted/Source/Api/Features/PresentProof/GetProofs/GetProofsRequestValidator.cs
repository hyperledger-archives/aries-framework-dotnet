namespace BlazorHosted.Features.PresentProofs
{
  using FluentValidation;
  
  public class GetProofsRequestValidator : AbstractValidator<GetProofsRequest>
  {

    public GetProofsRequestValidator()
    {

    }
  }
}