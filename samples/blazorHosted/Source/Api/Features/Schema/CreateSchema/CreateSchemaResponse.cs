namespace BlazorHosted.Features.Schemas
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class CreateSchemaResponse : BaseResponse
  {
    public CreateSchemaResponse() { }

    public CreateSchemaResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
