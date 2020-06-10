namespace BlazorHosted.Features.Connections
{
  using System;
  using System.Collections.Generic;
  using BlazorHosted.Features.Bases;

  public class CreateInvitationResponse : BaseResponse
  {
    /// <summary>
    /// a default constructor is required for deserialization
    /// </summary>
    public CreateInvitationResponse() { }

    public CreateInvitationResponse(Guid aRequestId)
    {
      RequestId = aRequestId;
    }
  }
}
