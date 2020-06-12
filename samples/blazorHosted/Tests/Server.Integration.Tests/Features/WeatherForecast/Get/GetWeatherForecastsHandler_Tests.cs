namespace GetWeatherForecastsHandler
{
  using BlazorHosted.Features.WeatherForecasts;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Server;
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Text.Json;
  using System.Threading.Tasks;

  public class Handle_Returns : BaseTest
  {
    private readonly GetWeatherForecastsRequest GetWeatherForecastsRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetWeatherForecastsRequest = new GetWeatherForecastsRequest { Days = 10 };
    }

    public async Task _10WeatherForecasts_Given_10DaysRequested()
    {
      GetWeatherForecastsResponse getWeatherForecastsResponse = await Send(GetWeatherForecastsRequest);

      ValidateGetWeatherForecastsResponse(getWeatherForecastsResponse);
    }

    private void ValidateGetWeatherForecastsResponse(GetWeatherForecastsResponse aGetWeatherForecastsResponse)
    {
      aGetWeatherForecastsResponse.RequestId.Should().Be(GetWeatherForecastsRequest.RequestId);
      aGetWeatherForecastsResponse.WeatherForecasts.Count.Should().Be(GetWeatherForecastsRequest.Days);
    }

  }
}