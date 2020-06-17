namespace BlazorHosted.Features.Schemas
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class CreateSchemaEndpoint : BaseEndpoint<CreateSchemaRequest, CreateSchemaResponse>
  {
    /// <summary>
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <param name="aCreateSchemaRequest"><see cref="CreateSchemaRequest"/></param>
    /// <returns><see cref="CreateSchemaResponse"/></returns>
    [HttpGet(CreateSchemaRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(CreateSchemaResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(CreateSchemaRequest aCreateSchemaRequest) => await Send(aCreateSchemaRequest);
  }
}
