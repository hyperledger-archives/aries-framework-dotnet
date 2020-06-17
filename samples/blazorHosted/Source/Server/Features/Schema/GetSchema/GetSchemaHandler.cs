namespace BlazorHosted.Features.Schemas
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class GetSchemaHandler : IRequestHandler<GetSchemaRequest, GetSchemaResponse>
  {

    public async Task<GetSchemaResponse> Handle
    (
      GetSchemaRequest aGetSchemaRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new GetSchemaResponse(aGetSchemaRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
