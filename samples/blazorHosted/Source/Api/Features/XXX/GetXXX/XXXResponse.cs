namespace BlazorHosted.Features.XXXs
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class XXXResponse : BaseResponse
  {
    public List<XXXDto> XXXs { get; set; }

    /// <summary>
    /// a default constructor is required for deserialization
    /// </summary>
    public XXXResponse() { }

    public XXXResponse(Guid aRequestId)
    {
      XXXs = new List<XXXDto>();
      RequestId = aRequestId;
    }
  }
}
