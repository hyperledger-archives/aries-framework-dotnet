namespace BlazorHosted.Features.Schemas
{
  using FluentValidation;

  public class GetSchemasRequestValidator : AbstractValidator<GetSchemasRequest>
  {
    public GetSchemasRequestValidator() { }
  }
}
