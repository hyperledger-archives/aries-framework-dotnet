namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class SendPingEndpoint : BaseEndpoint<SendPingRequest, SendPingResponse>
  {
    /// <summary>
    /// Send a trust ping to a connection
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aSendPingRequest"></param>
    /// <returns><see cref="SendPingResponse"/></returns>
    [HttpPost(SendPingRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(SendPingResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(SendPingRequest aSendPingRequest) => await Send(aSendPingRequest);
  }
}
