namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetCredentialRequest : BaseApiRequest, IRequest<GetCredentialResponse>
  {
    public const string Route = "api/Credentials/GetCredential";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}