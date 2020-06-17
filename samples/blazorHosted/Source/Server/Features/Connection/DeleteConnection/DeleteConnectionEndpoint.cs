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
    /// Your summary these comments will show in the Open API Docs
    /// </summary>
    /// <remarks>
    /// Longer Description
    /// </remarks>
    /// <param name="aDeleteConnectionRequest"></param>
    /// <returns><see cref="DeleteConnectionResponse"/></returns>
    [HttpGet(DeleteConnectionRequest.Route)]
    [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
    [ProducesResponseType(typeof(DeleteConnectionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Process(DeleteConnectionRequest aDeleteConnectionRequest) => await Send(aDeleteConnectionRequest);
  }
}
