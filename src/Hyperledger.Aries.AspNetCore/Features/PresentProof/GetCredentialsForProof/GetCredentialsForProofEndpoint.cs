namespace Hyperledger.Aries.AspNetCore.Features.PresentProofs
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetCredentialsForProofEndpoint : BaseEndpoint<GetCredentialsForProofRequest, GetCredentialsForProofResponse>
  {
    /// <summary>
    /// Get list of credentials for the specific proof request
    /// </summary>
    /// <param name="aGetCredentialsForProofRequest"><see cref="GetCredentialsForProofRequest"/></param>
    /// <returns><see cref="GetCredentialsForProofResponse"/></returns>
    [HttpGet(GetCredentialsForProofRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetCredentialsForProofResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetCredentialsForProofRequest aGetCredentialsForProofRequest) => await Send(aGetCredentialsForProofRequest);
  }
}
