namespace BlazorHosted.Features.IssueCredentials
{
  using BlazorHosted.Features.Bases;
  using Hyperledger.Aries.Features.IssueCredential;
  using MediatR;
  using System.Collections.Generic;

  public class OfferCredentialRequest : BaseApiRequest, IRequest<OfferCredentialResponse>
  {
    public const string RouteTemplate = "api/issue-credential/send-offer";

    /// <summary>
    /// The ConnectionId to use to send the offer
    /// </summary>
    /// <example>TODO</example>
    public string ConnectionId { get; set; } = null!;

    public string CredentialDefinitionId { get; set; } = null!;
    public List<CredentialPreviewAttribute> CredentialPreviewAttributes { get; set; } = null!;
    //public string SchemaId { get; set; } = null!;

    public OfferCredentialRequest()
    {
      CredentialPreviewAttributes = new List<CredentialPreviewAttribute>();
    }

    internal override string GetRoute() => RouteTemplate;
  }
}
