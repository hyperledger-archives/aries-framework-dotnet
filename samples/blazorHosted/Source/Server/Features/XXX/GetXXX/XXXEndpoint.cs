namespace BlazorHosted.Features.XXXs
{
  using Microsoft.AspNetCore.Mvc;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Bases;

  public class XXXEndpoint : BaseEndpoint<XXXRequest, XXXResponse>
  {
    [HttpGet(XXXRequest.Route)]
    public async Task<IActionResult> Process(XXXRequest aRequest) => await Send(aRequest);
  }
}
