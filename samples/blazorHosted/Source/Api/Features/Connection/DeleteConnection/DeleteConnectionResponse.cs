namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class DeleteConnectionResponse : BaseResponse
  {
    public DeleteConnectionResponse() { }

    public DeleteConnectionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
