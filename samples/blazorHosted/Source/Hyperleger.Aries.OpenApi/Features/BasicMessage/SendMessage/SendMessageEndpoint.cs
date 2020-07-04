namespace Hyperledger.Aries.OpenApi.Features.BasicMessaging
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class SendMessageEndpoint : BaseEndpoint<SendMessageRequest, SendMessageResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
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
