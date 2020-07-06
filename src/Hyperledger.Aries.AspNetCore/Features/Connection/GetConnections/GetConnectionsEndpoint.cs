namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetConnectionsEndpoint : BaseEndpoint<GetConnectionsRequest, GetConnectionsResponse>
  {
    /// <summary>
    /// Return all ConnectionRecords 
    /// </summary>
    /// <param name="aGetConnectionsRequest"><see cref="GetConnectionsRequest"/></param>
    /// <returns><see cref="GetConnectionsResponse"/></returns>
    [HttpGet(GetConnectionsRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetConnectionsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetConnectionsRequest aGetConnectionsRequest) => await Send(aGetConnectionsRequest);
  }
}
