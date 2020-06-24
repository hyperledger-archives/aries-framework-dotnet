namespace BlazorHosted.Features.IssueCredentials
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class OfferCredentialEndpoint : BaseEndpoint<OfferCredentialRequest, OfferCredentialResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aOfferCredentialRequest"><see cref="OfferCredentialRequest"/></param>
    /// <returns><see cref="OfferCredentialResponse"/></returns>
    [HttpGet(OfferCredentialRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(OfferCredentialResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(OfferCredentialRequest aOfferCredentialRequest) => await Send(aOfferCredentialRequest);
  }
}
