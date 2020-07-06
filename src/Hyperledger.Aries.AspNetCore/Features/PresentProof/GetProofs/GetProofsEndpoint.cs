namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetProofsEndpoint : BaseEndpoint<GetProofsRequest, GetProofsResponse>
  {
    /// <summary>
    /// Get all Proof Request records
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
