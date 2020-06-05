namespace BlazorHosted.Features.WeatherForecasts
{
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetWeatherForecastsEndpoint : BaseEndpoint<GetWeatherForecastsRequest, GetWeatherForecastsResponse>
  {
    /// <summary>
    /// Gets number of days <see cref="GetWeatherForecastsRequest.Days"/>
    /// </summary>
    /// <param name="aRequest"></param>
    /// <returns><see cref="GetWeatherForecastsResponse"/></returns>
    [HttpGet(GetWeatherForecastsRequest.Route)]
    public async Task<IActionResult> Process(GetWeatherForecastsRequest aGetWeatherForecastsRequest) => 
      await Send(aGetWeatherForecastsRequest);
  }
}
