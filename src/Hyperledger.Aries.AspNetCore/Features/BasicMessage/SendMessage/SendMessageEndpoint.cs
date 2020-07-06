namespace Hyperledger.Aries.AspNetCore.Features.BasicMessaging
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class SendMessageEndpoint : BaseEndpoint<SendMessageRequest, SendMessageResponse>
  {
    /// <summary>
    /// Send a message to a connection
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aSendMessageRequest"></param>
    /// <returns><see cref="SendMessageResponse"/></returns>
    [HttpPost(SendMessageRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(SendMessageResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] SendMessageRequest aSendMessageRequest) => await Send(aSendMessageRequest);
  }
}
