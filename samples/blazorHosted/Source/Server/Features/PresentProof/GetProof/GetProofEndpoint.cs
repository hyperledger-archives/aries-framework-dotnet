namespace BlazorHosted.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetProofEndpoint : BaseEndpoint<GetProofRequest, GetProofResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
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