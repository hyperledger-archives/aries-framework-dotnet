namespace BlazorHosted.Features.Schemas
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class GetSchemasEndpoint : BaseEndpoint<GetSchemasRequest, GetSchemasResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aGetSchemasRequest"><see cref="GetSchemasRequest"/></param>
    /// <returns><see cref="GetSchemasResponse"/></returns>
    [HttpGet(GetSchemasRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetSchemasResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetSchemasRequest aGetSchemasRequest) => await Send(aGetSchemasRequest);
  }
}
