namespace Hyperledger.Aries.AspNetCore.Features.IssueCredentials
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;
  using System;

  public class OfferCredentialEndpoint : BaseEndpoint<OfferCredentialRequest, OfferCredentialResponse>
  {
    /// <summary>
    /// Send holder a credential offer, independent of any proposal
    /// </summary>
    /// <param name="aOfferCredentialRequest"><see cref="OfferCredentialRequest"/></param>
    /// <returns><see cref="OfferCredentialResponse"/></returns>
    [HttpPost(OfferCredentialRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(OfferCredentialResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] OfferCredentialRequest aOfferCredentialRequest)
    {
      return await Send(aOfferCredentialRequest);
    }
  }
}
