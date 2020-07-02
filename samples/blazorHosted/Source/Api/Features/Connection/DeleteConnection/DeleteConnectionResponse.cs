namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;
  using System;

  public class DeleteConnectionResponse : BaseResponse
  {
    public DeleteConnectionResponse() { }

    public DeleteConnectionResponse(Guid aCorrelationId) : base(aCorrelationId) { }
  }
}
