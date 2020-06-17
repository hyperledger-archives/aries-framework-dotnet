namespace BlazorHosted.Features.Schemas
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  
  public class CreateSchemaHandler : IRequestHandler<CreateSchemaRequest, CreateSchemaResponse>
  {

    public async Task<CreateSchemaResponse> Handle
    (
      CreateSchemaRequest aCreateSchemaRequest,
      CancellationToken aCancellationToken
    )
    {
      var response = new CreateSchemaResponse(aCreateSchemaRequest.CorrelationId);

      return await Task.Run(() => response);
    }
  }
}
