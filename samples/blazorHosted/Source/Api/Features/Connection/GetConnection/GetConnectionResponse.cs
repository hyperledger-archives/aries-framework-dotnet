namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetConnectionResponse : BaseResponse
  {
    public GetConnectionResponse() { }

    public GetConnectionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
