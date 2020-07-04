namespace Hyperledger.Aries.OpenApi.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.OpenApi.Features.Bases;

  public class GetProofsEndpoint : BaseEndpoint<GetProofsRequest, GetProofsResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aGetProofsRequest"><see cref="GetProofsRequest"/></param>
    /// <returns><see cref="GetProofsResponse"/></returns>
    [HttpGet(GetProofsRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetProofsResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetProofsRequest aGetProofsRequest) => await Send(aGetProofsRequest);
  }
}
