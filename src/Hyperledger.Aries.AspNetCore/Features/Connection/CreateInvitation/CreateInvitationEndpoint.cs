namespace Hyperledger.Aries.AspNetCore.Features.Connections
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;

  public class CreateInvitationEndpoint : BaseEndpoint<CreateInvitationRequest, CreateInvitationResponse>
  {
    /// <summary>
    /// Create a ConnectionInvitationMessage and store corresponding ConnectionRecord in Wallet
    /// </summary>
    /// <param name="aCreateInvitationRequest"><see cref="CreateInvitationRequest"/></param>
    /// <returns><see cref="CreateInvitationResponse"/></returns>
    [HttpPost(CreateInvitationRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateInvitationResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] CreateInvitationRequest aCreateInvitationRequest) => 
      await Send(aCreateInvitationRequest);
  }
}
