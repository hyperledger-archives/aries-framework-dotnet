namespace BlazorHosted.Features.Schemas
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetSchemaResponse : BaseResponse
  {
    public GetSchemaResponse() { }

    public GetSchemaResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
