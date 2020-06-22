namespace BlazorHosted.Features.WeatherForecasts
{
  using MediatR;
  using BlazorHosted.Features.Bases;

  public class GetWeatherForecastsRequest : BaseApiRequest, IRequest<GetWeatherForecastsResponse>
  {
    public const string Route = "api/weatherForecasts";

    /// <summary>
    /// The Number of days of forecasts to get
    /// </summary>
    /// <example>5</example>
    public int Days { get; set; }

    internal override string GetRoute() => $"{Route}?{nameof(Days)}={Days}&{nameof(CorrelationId)}={CorrelationId}";
  }
}
