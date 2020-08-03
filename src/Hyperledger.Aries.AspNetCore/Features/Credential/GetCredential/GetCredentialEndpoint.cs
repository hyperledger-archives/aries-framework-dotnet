namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetCredentialEndpoint : BaseEndpoint<GetCredentialRequest, GetCredentialResponse>
  {
    /// <summary>
    /// Gets the credential by its Id
    /// </summary>
    /// <param name="aGetCredentialRequest"><see cref="GetCredentialRequest"/></param>
    /// <returns><see cref="GetCredentialResponse"/></returns>
    [HttpGet(GetCredentialRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialRequest aGetCredentialRequest) => await Send(aGetCredentialRequest);
  }
}
