namespace BlazorHosted.Features.Credentials
{
  using BlazorHosted.Features.Bases;
  using BlazorHosted.Features.CredentialDefinitions;

  internal partial class CredentialState
  {
    public class OfferCredentialAction : BaseAction
    {
      public OfferCredentialRequest OfferCredentialRequest { get; set; }
    }
  }
}
