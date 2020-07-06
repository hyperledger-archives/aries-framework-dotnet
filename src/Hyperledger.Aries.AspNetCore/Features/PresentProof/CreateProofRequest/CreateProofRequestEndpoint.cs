namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class CreateProofRequestEndpoint : BaseEndpoint<CreateProofRequestRequest, CreateProofRequestResponse>
  {
    /// <summary>
    /// Create a Request for Proof
    /// </summary>
    /// <param name="aCreateProofRequestRequest"><see cref="CreateProofRequestRequest"/></param>
    /// <returns><see cref="CreateProofRequestResponse"/></returns>
    [HttpPost(CreateProofRequestRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateProofRequestResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody]CreateProofRequestRequest aCreateProofRequestRequest) => await Send(aCreateProofRequestRequest);
  }
}
