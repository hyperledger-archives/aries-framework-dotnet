namespace BlazorHosted.Features.Connections
{
  using Microsoft.AspNetCore.Mvc;
  using Swashbuckle.AspNetCore.Annotations;
  using System.Net;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class DeleteConnectionEndpoint : BaseEndpoint<DeleteConnectionRequest, DeleteConnectionResponse>
  {
    /// <summary>
    /// Delete a connection record
    /// </summary>
    /// <param name="aDeleteConnectionRequest"><see cref="DeleteConnectionRequest"/></param>
    /// <returns><see cref="DeleteConnectionResponse"/></returns>
    [HttpDelete(DeleteConnectionRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(DeleteConnectionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(DeleteConnectionRequest aDeleteConnectionRequest) => await Send(aDeleteConnectionRequest);
  }
}
