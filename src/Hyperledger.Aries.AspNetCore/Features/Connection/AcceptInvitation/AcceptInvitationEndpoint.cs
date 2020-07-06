namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class AcceptInvitationEndpoint : BaseEndpoint<AcceptInvitationRequest, AcceptInvitationResponse>
  {
    /// <summary>
    /// Accept a connection invitation
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aAcceptInvitationRequest"></param>
    /// <returns><see cref="AcceptInvitationResponse"/></returns>
    [HttpPost(AcceptInvitationRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(AcceptInvitationResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] AcceptInvitationRequest aAcceptInvitationRequest) => await Send(aAcceptInvitationRequest);
  }
}
