namespace BlazorHosted.Features.Connections
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class DeleteConnectionRequest : BaseApiRequest, IRequest<DeleteConnectionResponse>
  {
    public const string Route = "api/Connections/DeleteConnection";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}