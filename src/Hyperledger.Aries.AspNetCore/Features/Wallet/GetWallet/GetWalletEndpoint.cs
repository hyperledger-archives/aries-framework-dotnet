namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetWalletEndpoint : BaseEndpoint<GetWalletRequest, GetWalletResponse>
  {
    /// <summary>
    /// Returns the Provisioning information for the agent.
    /// </summary>
    /// <param name="aGetWalletRequest"></param>
    [HttpGet(GetWalletRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetWalletResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetWalletRequest aGetWalletRequest) => await Send(aGetWalletRequest);
  }
}
