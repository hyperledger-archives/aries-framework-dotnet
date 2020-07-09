namespace Hyperledger.Aries.AspNetCore.Features.Wallets
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class ResetWalletEndpoint : BaseEndpoint<ResetWalletRequest, ResetWalletResponse>
  {
    /// <summary>
    /// Caution! All Data will be lost.
    /// Delete and Create New Wallet and reprovision the Agent.
    /// </summary>
    /// <param name="aResetWalletRequest"><see cref="ResetWalletRequest"/></param>
    /// <returns><see cref="ResetWalletResponse"/></returns>
    [HttpPost(ResetWalletRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(ResetWalletResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody] ResetWalletRequest aResetWalletRequest) => await Send(aResetWalletRequest);
  }
}
