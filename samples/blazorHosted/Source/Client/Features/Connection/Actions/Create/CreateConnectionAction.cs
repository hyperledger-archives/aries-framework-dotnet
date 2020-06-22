﻿namespace BlazorHosted.Features.Connections
{
  using BlazorHosted.Features.Bases;

  internal partial class ConnectionState
  {
    public class CreateConnectionAction : BaseAction 
    {
      public CreateInvitationRequest CreateInvitationRequest { get; set; }
    }
  }
}
