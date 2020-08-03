namespace Hyperledger.Aries.AspNetCore.Features.Schemas
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Bases;

  public class CreateSchemaEndpoint : BaseEndpoint<CreateSchemaRequest, CreateSchemaResponse>
  {
    /// <summary>
    /// Creates new schema
    /// </summary>
    /// <param name="aCreateSchemaRequest"><see cref="CreateSchemaRequest"/></param>
    /// <returns><see cref="CreateSchemaResponse"/></returns>
    [HttpPost(CreateSchemaRequest.RouteTemplate)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateSchemaResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process([FromBody]CreateSchemaRequest aCreateSchemaRequest) => 
      await Send(aCreateSchemaRequest);
  }
}
