namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetCredentialDefinitionsEndpoint : BaseEndpoint<GetCredentialDefinitionsRequest, GetCredentialDefinitionsResponse>
  {
    /// <summary>
    /// Get List of Credential Definitions
    /// </summary>
    /// <param name="aGetCredentialDefinitionsRequest"><see cref="GetCredentialDefinitionsRequest"/></param>
    /// <returns><see cref="GetCredentialDefinitionsResponse"/></returns>
    [HttpGet(GetCredentialDefinitionsRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialDefinitionsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialDefinitionsRequest aGetCredentialDefinitionsRequest) => await Send(aGetCredentialDefinitionsRequest);
  }
}
