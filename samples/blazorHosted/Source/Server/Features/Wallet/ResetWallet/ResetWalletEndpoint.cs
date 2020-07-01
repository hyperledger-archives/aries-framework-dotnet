namespace BlazorHosted.Features.Wallets
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class ResetWalletEndpoint : BaseEndpoint<ResetWalletRequest, ResetWalletResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
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
