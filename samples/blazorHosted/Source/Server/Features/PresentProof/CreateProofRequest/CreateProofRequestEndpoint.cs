namespace BlazorHosted.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class CreateProofRequestEndpoint : BaseEndpoint<CreateProofRequestRequest, CreateProofRequestResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aSendRequestForProofRequest"><see cref="CreateProofRequestRequest"/></param>
    /// <returns><see cref="CreateProofRequestResponse"/></returns>
    [HttpPost(CreateProofRequestRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateProofRequestResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody]CreateProofRequestRequest aSendRequestForProofRequest) => await Send(aSendRequestForProofRequest);
  }
}
