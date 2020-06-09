namespace BlazorHosted.Features.Healths
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetHealthEndpoint : BaseEndpoint<GetHealthRequest, GetHealthResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// `<see cref="GetHealthRequest.Days"/>`
    /// </remarks>
    /// <param name="aGetHealthRequest"></param>
    /// <returns><see cref="GetHealthResponse"/></returns>
    [HttpGet(GetHealthRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetHealthResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetHealthRequest aGetHealthRequest) => await Send(aGetHealthRequest);
  }
}
