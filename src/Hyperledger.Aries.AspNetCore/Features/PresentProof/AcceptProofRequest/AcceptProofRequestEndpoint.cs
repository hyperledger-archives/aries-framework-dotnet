namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class AcceptProofRequestEndpoint : BaseEndpoint<AcceptProofRequestRequest, AcceptProofRequestResponse>
  {
    /// <summary>
    /// Accept proof request
    /// </summary>
    /// <param name="aAcceptProofRequestRequest"><see cref="AcceptProofRequestRequest"/></param>
    /// <returns><see cref="AcceptProofRequestResponse"/></returns>
    [HttpGet(AcceptProofRequestRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(AcceptProofRequestResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(AcceptProofRequestRequest aAcceptProofRequestRequest) => await Send(aAcceptProofRequestRequest);
  }
}
