namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class RemoveCredentialEndpoint : BaseEndpoint<RemoveCredentialRequest, RemoveCredentialResponse>
  {
    /// <summary>
    /// Remove a credential from the wallet by id
    /// </summary>
    /// <param name="aRemoveCredentialRequest"><see cref="RemoveCredentialRequest"/></param>
    /// <returns><see cref="RemoveCredentialResponse"/></returns>
    [HttpPost(RemoveCredentialRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(RemoveCredentialResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] RemoveCredentialRequest aRemoveCredentialRequest) => await Send(aRemoveCredentialRequest);
  }
}
