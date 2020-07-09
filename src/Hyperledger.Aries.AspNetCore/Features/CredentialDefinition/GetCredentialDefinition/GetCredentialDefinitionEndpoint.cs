namespace Hyperledger.Aries.AspNetCore.Features.CredentialDefinitions
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetCredentialDefinitionEndpoint : BaseEndpoint<GetCredentialDefinitionRequest, GetCredentialDefinitionResponse>
  {
    /// <summary>
    /// Gets a credential definition from the ledger
    /// </summary>
    /// <param name="aGetCredentialDefinitionRequest"><see cref="GetCredentialDefinitionRequest"/></param>
    /// <returns><see cref="GetCredentialDefinitionResponse"/></returns>
    [HttpGet(GetCredentialDefinitionRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialDefinitionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialDefinitionRequest aGetCredentialDefinitionRequest) => await Send(aGetCredentialDefinitionRequest);
  }
}
