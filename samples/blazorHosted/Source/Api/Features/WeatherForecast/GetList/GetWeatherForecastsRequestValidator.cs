namespace BlazorHosted.Features.WeatherForecasts
{
  using FluentValidation;

  public class GetWeatherForecastsRequestValidator : AbstractValidator<GetWeatherForecastsRequest>
  {

    // Inject Dependencies via contstructor
    public GetWeatherForecastsRequestValidator()
    {
      RuleFor(aGetWeatherForecastRequest => aGetWeatherForecastRequest.Days)
        .GreaterThan(0);
    }
  }
}