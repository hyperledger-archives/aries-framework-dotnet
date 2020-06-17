namespace BlazorHosted.Features.Credentials
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetCredentialsRequest : BaseApiRequest, IRequest<GetCredentialsResponse>
  {
    public const string Route = "api/Credentials/GetCredentials";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}