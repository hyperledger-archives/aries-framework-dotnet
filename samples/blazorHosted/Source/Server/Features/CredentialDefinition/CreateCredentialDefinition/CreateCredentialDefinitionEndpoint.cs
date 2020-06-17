namespace BlazorHosted.Features.CredentialDefinitions
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class CreateCredentialDefinitionEndpoint : BaseEndpoint<CreateCredentialDefinitionRequest, CreateCredentialDefinitionResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aCreateCredentialDefinitionRequest"><see cref="CreateCredentialDefinitionRequest"/></param>
    /// <returns><see cref="CreateCredentialDefinitionResponse"/></returns>
    [HttpGet(CreateCredentialDefinitionRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateCredentialDefinitionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(CreateCredentialDefinitionRequest aCreateCredentialDefinitionRequest) => await Send(aCreateCredentialDefinitionRequest);
  }
}
