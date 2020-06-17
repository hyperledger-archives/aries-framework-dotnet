namespace BlazorHosted.Features.Credentials
{
  using FluentValidation;

  public class GetCredentialsRequestValidator : AbstractValidator<GetCredentialsRequest>
  {
    public GetCredentialsRequestValidator() { }
  }
}
