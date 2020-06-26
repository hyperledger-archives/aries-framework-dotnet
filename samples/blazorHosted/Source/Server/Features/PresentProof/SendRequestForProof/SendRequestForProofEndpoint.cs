namespace BlazorHosted.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class SendRequestForProofEndpoint : BaseEndpoint<SendRequestForProofRequest, SendRequestForProofResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aSendRequestForProofRequest"><see cref="SendRequestForProofRequest"/></param>
    /// <returns><see cref="SendRequestForProofResponse"/></returns>
    [HttpGet(SendRequestForProofRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(SendRequestForProofResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(SendRequestForProofRequest aSendRequestForProofRequest) => await Send(aSendRequestForProofRequest);
  }
}
