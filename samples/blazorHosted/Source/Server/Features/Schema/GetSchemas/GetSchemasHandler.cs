namespace BlazorHosted.Features.Schemas
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetSchemasHandler : IRequestHandler<GetSchemasRequest, GetSchemasResponse>
  {

    public async Task<GetSchemasResponse> Handle
    (
      GetSchemasRequest aGetSchemasRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetSchemasResponse(aGetSchemasRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
