namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;
  using System;

  public class GetCredentialResponse : BaseResponse
  {
    public CredentialRecord? CredentialRecord { get; set; }

    public GetCredentialResponse() { }

    public GetCredentialResponse(Guid aCorrelationId, CredentialRecord aCredentialRecord) : base(aCorrelationId)
    {
      CredentialRecord = aCredentialRecord;
    }
  }
}
