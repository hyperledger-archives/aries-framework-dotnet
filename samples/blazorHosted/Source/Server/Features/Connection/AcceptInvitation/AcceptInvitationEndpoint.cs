namespace BlazorHosted.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class AcceptInvitationEndpoint : BaseEndpoint<AcceptInvitationRequest, AcceptInvitationResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aAcceptInvitationRequest"></param>
    /// <returns><see cref="AcceptInvitationResponse"/></returns>
    [HttpGet(AcceptInvitationRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(AcceptInvitationResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(AcceptInvitationRequest aAcceptInvitationRequest) => await Send(aAcceptInvitationRequest);
  }
}
