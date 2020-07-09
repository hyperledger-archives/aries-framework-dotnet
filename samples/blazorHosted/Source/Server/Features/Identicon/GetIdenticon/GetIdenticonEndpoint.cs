namespace Hyperledger.Aries.AspNetCore.Features.Identicons
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using Jdenticon.AspNetCore;

  public class GetIdenticonEndpoint : ControllerBase
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    [HttpGet("api/identicon")]
    [SwaggerOperation(Tags = new[] { "Identicon" })]
    [ProducesResponseType(typeof(IdenticonResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    protected IActionResult Get(string aValue, int aSize) => IdenticonResult.FromValue(aValue, aSize);
  }
}
