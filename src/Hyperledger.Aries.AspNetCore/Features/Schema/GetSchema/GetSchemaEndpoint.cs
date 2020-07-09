namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class GetSchemaEndpoint : BaseEndpoint<GetSchemaRequest, GetSchemaResponse>
  {
    /// <summary>
    /// Returns the Schema with requested <see cref="GetSchemaRequest.SchemaId"/>
    /// </summary>
    /// <param name="aGetSchemaRequest"><see cref="GetSchemaRequest"/></param>
    /// <returns><see cref="GetSchemaResponse"/></returns>
    [HttpGet(GetSchemaRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(GetSchemaResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(GetSchemaRequest aGetSchemaRequest) => await Send(aGetSchemaRequest);
  }
}
