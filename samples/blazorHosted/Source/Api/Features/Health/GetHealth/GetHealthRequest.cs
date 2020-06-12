namespace BlazorHosted.Features.Healths
{
  using MediatR;
  using System.Text.Json.Serialization;
  using BlazorHosted.Features.Bases;

  public class GetHealthRequest : BaseApiRequest, IRequest<GetHealthResponse>
  {
    public const string Route = "api/Health/GetHealth";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string RouteFactory => $"{Route}?{nameof(Days)}={Days}&{nameof(RequestId)}={RequestId}";
  }
}