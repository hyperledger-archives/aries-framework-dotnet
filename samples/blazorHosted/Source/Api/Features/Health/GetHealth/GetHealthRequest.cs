namespace BlazorHosted.Features.Healths
{
  using BlazorHosted.Features.Bases;
  using MediatR;

  public class GetHealthRequest : BaseApiRequest, IRequest<GetHealthResponse>
  {
    public const string Route = "api/Health/GetHealth";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string GetRoute() => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}
