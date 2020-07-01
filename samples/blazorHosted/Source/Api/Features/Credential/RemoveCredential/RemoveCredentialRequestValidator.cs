﻿namespace BlazorHosted.Features.Credentials
{
  using FluentValidation;

  public class RemoveCredentialRequestValidator : AbstractValidator<RemoveCredentialRequest>
  {
    public RemoveCredentialRequestValidator()
    {
      RuleFor(aRemoveCredentialRequest => aRemoveCredentialRequest.CredentialId)
        .NotEmpty();
    }
  }
}