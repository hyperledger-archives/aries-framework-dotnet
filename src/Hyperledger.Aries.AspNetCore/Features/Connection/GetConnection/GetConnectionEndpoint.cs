namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetConnectionEndpoint : BaseEndpoint<GetConnectionRequest, GetConnectionResponse>
  {
    /// <summary>
    /// Return a single ConnectionRecord
    /// </summary>
    /// <param name="aGetConnectionRequest"><see cref="GetConnectionRequest"/></param>
    /// <returns><see cref="GetConnectionResponse"/></returns>
    [HttpGet(GetConnectionRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetConnectionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetConnectionRequest aGetConnectionRequest) => await Send(aGetConnectionRequest);
  }
}
