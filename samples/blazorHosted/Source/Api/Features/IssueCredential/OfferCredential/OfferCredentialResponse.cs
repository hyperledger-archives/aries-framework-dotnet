namespace BlazorHosted.Features.IssueCredentials
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;
  using System;

  public class OfferCredentialResponse : BaseResponse
  {
    public CredentialOfferMessage CredentialOfferMessage { get; set; } = null!;

    public OfferCredentialResponse() { }

    public OfferCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
