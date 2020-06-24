namespace BlazorHosted.Features.IssueCredentials
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class OfferCredentialRequest : BaseApiRequest, IRequest<OfferCredentialResponse>
  {
    public const string Route = "api/IssueCredentials/OfferCredential";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}