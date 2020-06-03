namespace blazorhosted.Features.WeatherForecasts
{
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;
  using blazorhosted.Features.Bases;

  public class GetWeatherForecastsEndpoint : BaseEndpoint<GetWeatherForecastsRequest, GetWeatherForecastsResponse>
  {
    [HttpGet(GetWeatherForecastsRequest.Route)]
    public async Task<IActionResult> Process(GetWeatherForecastsRequest aRequest) => await Send(aRequest);
  }
}
