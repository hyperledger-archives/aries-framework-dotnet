namespace BlazorHosted.Features.Healths
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class GetHealthResponse : BaseResponse
  {
    /// <summary>
    /// a default constructor is required for deserialization
    /// </summary>
    public GetHealthResponse() { }

    public GetHealthResponse(Guid aRequestId)
    {
      CorrelationId = aRequestId;
    }
  }
}
