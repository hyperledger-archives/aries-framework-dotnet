namespace BlazorHosted.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class RecieveInvitationEndpoint : BaseEndpoint<RecieveInvitationRequest, RecieveInvitationResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aRecieveInvitationRequest"></param>
    /// <returns><see cref="RecieveInvitationResponse"/></returns>
    [HttpPost(RecieveInvitationRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(RecieveInvitationResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] RecieveInvitationRequest aRecieveInvitationRequest) => await Send(aRecieveInvitationRequest);
  }
}
