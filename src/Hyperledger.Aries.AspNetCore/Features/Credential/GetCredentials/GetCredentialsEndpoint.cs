namespace Hyperledger.Aries.AspNetCore.Features.Credentials
{
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;

  public class GetCredentialsEndpoint : BaseEndpoint<GetCredentialsRequest, GetCredentialsResponse>
  {
    /// <summary>
    /// Lists the credentials
    /// </summary>
    /// <param name="aGetCredentialsRequest"><see cref="GetCredentialsRequest"/></param>
    /// <returns><see cref="GetCredentialsResponse"/></returns>
    [HttpGet(GetCredentialsRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialsRequest aGetCredentialsRequest) => await Send(aGetCredentialsRequest);
  }
}
