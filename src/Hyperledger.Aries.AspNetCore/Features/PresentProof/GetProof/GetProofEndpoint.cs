namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetProofEndpoint : BaseEndpoint<GetProofRequest, GetProofResponse>
  {
    /// <summary>
    /// Get a specific proof request record
    /// </summary>
    /// <param name="aGetProofRequest"><see cref="GetProofRequest"/></param>
    /// <returns><see cref="GetProofResponse"/></returns>
    [HttpGet(GetProofRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetProofResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetProofRequest aGetProofRequest) => await Send(aGetProofRequest);
  }
}
