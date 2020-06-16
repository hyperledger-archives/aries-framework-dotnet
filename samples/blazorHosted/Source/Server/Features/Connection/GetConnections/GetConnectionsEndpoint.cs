namespace BlazorHosted.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetConnectionsEndpoint : BaseEndpoint<GetConnectionsRequest, GetConnectionsResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aGetConnectionsRequest"></param>
    /// <returns><see cref="GetConnectionsResponse"/></returns>
    [HttpGet(GetConnectionsRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetConnectionsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetConnectionsRequest aGetConnectionsRequest) => await Send(aGetConnectionsRequest);
  }
}
