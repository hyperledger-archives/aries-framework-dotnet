namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetSchemasEndpoint : BaseEndpoint<GetSchemasRequest, GetSchemasResponse>
  {
    /// <summary>
    /// Lists the known schemas
    /// </summary>
    /// <param name="aGetSchemasRequest"><see cref="GetSchemasRequest"/></param>
    /// <returns><see cref="GetSchemasResponse"/></returns>
    [HttpGet(GetSchemasRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetSchemasResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetSchemasRequest aGetSchemasRequest) => await Send(aGetSchemasRequest);
  }
}
