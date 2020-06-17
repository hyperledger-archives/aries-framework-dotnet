namespace BlazorHosted.Features.Schemas
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetSchemasResponse : BaseResponse
  {
    public GetSchemasResponse() { }

    public GetSchemasResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
