namespace BlazorHosted.Features.Credentials
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class RemoveCredentialEndpoint : BaseEndpoint<RemoveCredentialRequest, RemoveCredentialResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aRemoveCredentialRequest"><see cref="RemoveCredentialRequest"/></param>
    /// <returns><see cref="RemoveCredentialResponse"/></returns>
    [HttpGet(RemoveCredentialRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(RemoveCredentialResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(RemoveCredentialRequest aRemoveCredentialRequest) => await Send(aRemoveCredentialRequest);
  }
}
