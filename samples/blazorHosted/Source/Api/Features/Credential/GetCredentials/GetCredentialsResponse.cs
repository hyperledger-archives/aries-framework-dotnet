namespace BlazorHosted.Features.Credentials
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;

  public class GetCredentialsResponse : BaseResponse
  {
    public List<CredentialRecord> CredentialRecords { get; set; } = null!;
    public GetCredentialsResponse() { }

    public GetCredentialsResponse(Guid aCorrelationId, List<CredentialRecord> aCredentialRecords) : base(aCorrelationId) 
    {
      CredentialRecords = aCredentialRecords;
    }
  }
}
