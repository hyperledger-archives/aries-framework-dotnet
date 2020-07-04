namespace Hyperledger.Aries.OpenApi.Features.Credentials
{
  using Hyperledger.Aries.OpenApi.Features.Bases;
  using Hyperledger.Aries.OpenApi.Features.IssueCredentials;

  internal partial class CredentialState
  {
    public class OfferCredentialAction : BaseAction
    {
      public OfferCredentialRequest OfferCredentialRequest { get; set; }
    }
  }
}
