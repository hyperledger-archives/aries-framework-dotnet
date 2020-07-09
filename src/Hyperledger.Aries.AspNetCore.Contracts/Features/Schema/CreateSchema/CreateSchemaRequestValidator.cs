namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using FluentValidation;

  public class CreateSchemaRequestValidator : AbstractValidator<CreateSchemaRequest>
  {
    public CreateSchemaRequestValidator()
    {
      RuleFor(aCreateSchemaRequest => aCreateSchemaRequest.Name)
        .NotEmpty();
      RuleFor(aCreateSchemaRequest => aCreateSchemaRequest.Version)
        .NotEmpty()
        .Matches("^[0-9.]+$");
      RuleForEach(aCreateSchemaRequest => aCreateSchemaRequest.AttributeNames)
        .NotEmpty();
    }
  }
}
