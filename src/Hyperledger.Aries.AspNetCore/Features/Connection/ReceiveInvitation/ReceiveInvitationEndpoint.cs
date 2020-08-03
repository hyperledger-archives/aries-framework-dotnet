namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class ReceiveInvitationEndpoint : BaseEndpoint<ReceiveInvitationRequest, ReceiveInvitationResponse>
  {
    /// <summary>
    /// Receive a new connection invitation
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aReceiveInvitationRequest"></param>
    /// <returns><see cref="ReceiveInvitationResponse"/></returns>
    [HttpPost(ReceiveInvitationRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(ReceiveInvitationResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] ReceiveInvitationRequest aReceiveInvitationRequest) => await Send(aReceiveInvitationRequest);
  }
}
