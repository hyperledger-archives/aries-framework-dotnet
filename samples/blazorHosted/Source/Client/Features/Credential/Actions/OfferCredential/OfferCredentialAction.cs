namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Hyperledger.Aries.AspNetCore.Features.IssueCredentials;

  internal partial class CredentialState
  {
    public class OfferCredentialAction : BaseAction
    {
      public OfferCredentialRequest OfferCredentialRequest { get; set; }
    }
  }
}
