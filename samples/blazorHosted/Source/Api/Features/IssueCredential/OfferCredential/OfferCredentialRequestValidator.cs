﻿namespace Hyperledger.Aries.OpenApi.Features.IssueCredentials
{
  using FluentValidation;
  
  public class OfferCredentialRequestValidator : AbstractValidator<OfferCredentialRequest>
  {

    public OfferCredentialRequestValidator()
    {
      //RuleFor(aOfferCredentialRequest => aOfferCredentialRequest.ConnectionId)
      //  .NotEmpty();
      //RuleFor(aOfferCredentialRequest => aOfferCredentialRequest.CredentialDefinitionId)
      //  .NotEmpty();
    }
  }
}