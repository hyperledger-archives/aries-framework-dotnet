namespace BlazorHosted.Features.Wallets
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetWalletEndpoint : BaseEndpoint<GetWalletRequest, GetWalletResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aGetWalletRequest"></param>
    /// <returns><see cref="GetWalletResponse"/></returns>
    [HttpGet(GetWalletRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetWalletResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetWalletRequest aGetWalletRequest) => await Send(aGetWalletRequest);
  }
}
