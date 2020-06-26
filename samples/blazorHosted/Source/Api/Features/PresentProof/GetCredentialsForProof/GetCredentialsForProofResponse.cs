namespace BlazorHosted.Features.PresentProofs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;

  public class GetCredentialsForProofResponse : BaseResponse
  {
    public GetCredentialsForProofResponse() { }

    public List<Credential> Credentials { get; set; } = null!;

    public GetCredentialsForProofResponse(List<Credential> aCredentials, Guid aCorrelationId) : base(aCorrelationId) 
    {
      Credentials = aCredentials;
    }
  }
}
