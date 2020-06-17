namespace BlazorHosted.Features.Credentials
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetCredentialsEndpoint : BaseEndpoint<GetCredentialsRequest, GetCredentialsResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aGetCredentialsRequest"></param>
    /// <returns><see cref="GetCredentialsResponse"/></returns>
    [HttpGet(GetCredentialsRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialsRequest aGetCredentialsRequest) => await Send(aGetCredentialsRequest);
  }
}
