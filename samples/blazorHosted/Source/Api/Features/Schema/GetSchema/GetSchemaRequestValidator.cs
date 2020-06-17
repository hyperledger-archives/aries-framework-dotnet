﻿namespace BlazorHosted.Features.Schemas
{
  using FluentValidation;

  public class GetSchemaRequestValidator : AbstractValidator<GetSchemaRequest>
  {
    public GetSchemaRequestValidator()
    {
      RuleFor(aGetSchemaRequest => aGetSchemaRequest.SchemaId)
        .NotEmpty();
    }
  }
}
