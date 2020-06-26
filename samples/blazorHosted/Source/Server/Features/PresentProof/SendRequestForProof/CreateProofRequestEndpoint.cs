namespace BlazorHosted.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class CreateProofRequestEndpoint : BaseEndpoint<CreateProofRequestRequest, CreateProofRequestRequestResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aSendRequestForProofRequest"><see cref="CreateProofRequestRequest"/></param>
    /// <returns><see cref="CreateProofRequestRequestResponse"/></returns>
    [HttpPost(CreateProofRequestRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateProofRequestRequestResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody]CreateProofRequestRequest aSendRequestForProofRequest) => await Send(aSendRequestForProofRequest);
  }
}
