namespace BlazorHosted.Features.Credentials
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;
  using System;

  public class GetCredentialResponse : BaseResponse
  {
    public CredentialRecord CredentialRecord { get; set; } = null!;

    public GetCredentialResponse() { }

    public GetCredentialResponse(Guid aCorrelationId, CredentialRecord aCredentialRecord) : base(aCorrelationId)
    {
      CredentialRecord = aCredentialRecord;
    }
  }
}
