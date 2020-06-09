namespace BlazorHosted.Features.WeatherForecasts
{
  using BlazorHosted.Features.Bases;
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;

  public class GetWeatherForecastsEndpoint : BaseEndpoint<GetWeatherForecastsRequest, GetWeatherForecastsResponse>
  {
    /// <summary>
    /// Get Weather Forecasts
    /// </summary>
    /// <remarks>
    /// Gets Weather Forecasts for the number of days specified in the request
    /// `<see cref="GetWeatherForecastsRequest.Days"/>`
    /// </remarks>
    /// <param name="aGetWeatherForecastsRequest"></param>
    /// <returns><see cref="GetWeatherForecastsResponse"/></returns>
    [HttpGet(GetWeatherForecastsRequest.Route)]
    [
      SwaggerOperation
      (
        Tags = new[] { FeatureAnnotations.FeatureGroup }, 
        Description = "Weather Forecasts", // <== doesn't seem to do anything
        Summary = "A Summary", // <== doesn't seem to do anything
        OperationId = "AF78591E-5AD6-47F4-9750-E2B90751737A" // <== doesn't seem to do anything
      ) 
    ]
    [ProducesResponseType(typeof(GetWeatherForecastsResponse), (int) HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromQuery] GetWeatherForecastsRequest aGetWeatherForecastsRequest) => 
      await Send(aGetWeatherForecastsRequest);
  }
}
