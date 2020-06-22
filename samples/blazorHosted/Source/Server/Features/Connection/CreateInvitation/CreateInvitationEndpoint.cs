namespace BlazorHosted.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class CreateInvitationEndpoint : BaseEndpoint<CreateInvitationRequest, CreateInvitationResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// `<see cref="CreateInvitationRequest"/>`
    /// </remarks>
    /// <param name="aCreateInvitationRequest"></param>
    /// <returns><see cref="CreateInvitationResponse"/></returns>
    [HttpPost(CreateInvitationRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateInvitationResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] CreateInvitationRequest aCreateInvitationRequest) => await Send(aCreateInvitationRequest);
  }
}
