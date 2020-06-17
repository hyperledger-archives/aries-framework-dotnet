namespace BlazorHosted.Features.CredentialDefinitions
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetCredentialDefinitionsEndpoint : BaseEndpoint<GetCredentialDefinitionsRequest, GetCredentialDefinitionsResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aGetCredentialDefinitionsRequest"><see cref="GetCredentialDefinitionsRequest"/></param>
    /// <returns><see cref="GetCredentialDefinitionsResponse"/></returns>
    [HttpGet(GetCredentialDefinitionsRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialDefinitionsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialDefinitionsRequest aGetCredentialDefinitionsRequest) => await Send(aGetCredentialDefinitionsRequest);
  }
}
