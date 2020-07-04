namespace Hyperledger.Aries.OpenApi.Features.IssueCredentials
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;
  using System;

  public class OfferCredentialResponse : BaseResponse
  {
    public CredentialOfferMessage CredentialOfferMessage { get; set; } = null!;

    public OfferCredentialResponse() { }

    public OfferCredentialResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
