namespace BlazorHosted.Features.WeatherForecasts
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetWeatherForecastsRequest : BaseApiRequest, IRequest<GetWeatherForecastsResponse>
  {
    public const string RouteTemplate = "api/weatherForecasts";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string GetRoute() => $"{RouteTemplate}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}
