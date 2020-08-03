namespace Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure
{
  using FluentAssertions;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.Features.DidExchange;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  public partial class BaseTest
  {
    internal static ReceiveInvitationRequest CreateValidReceiveInvitationRequest()
    {
      string encodedInvitationDetails =
        "https://localhost:5551?c_i=eyJsYWJlbCI6IkZhYmVyIiwiaW1hZ2VVcmwiOm51bGwsInNlcnZpY2VFbmRwb2ludCI6Imh0dHBzOi8vbG9jYWxob3N0OjU1NTEiLCJyb3V0aW5nS2V5cyI6WyJCRTlQaEFCa0dveDl3b1JYTURGWjVwWXZCc3BzU3VNdDFnUzJXbjJVU0RhViJdLCJyZWNpcGllbnRLZXlzIjpbIjlMWEdOQzh4SnJQUnI3V3NKR1lZclpmdDZjTWhtWjhEZ21LMldBcWMxTDk4Il0sIkBpZCI6IjQ2NzNkYjVjLTFkM2ItNGJlYy1hMDVhLTZiZWE4MTRiMTEwMiIsIkB0eXBlIjoiZGlkOnNvdjpCekNic05ZaE1yakhpcVpEVFVBU0hnO3NwZWMvY29ubmVjdGlvbnMvMS4wL2ludml0YXRpb24ifQ==";

      return new ReceiveInvitationRequest(aInvitationDetails: encodedInvitationDetails);
      
    }

    internal static void ValidateReceiveInvitationResponse
    (
      ReceiveInvitationRequest aReceiveInvitationRequest,
      ReceiveInvitationResponse aReceiveInvitationRespone
    )
    {
      aReceiveInvitationRespone.CorrelationId.Should().Be(aReceiveInvitationRequest.CorrelationId);
      
      ConnectionInvitationMessage connectionInvitationMessage = aReceiveInvitationRespone.ConnectionInvitationMessage;

      connectionInvitationMessage.Should().NotBeNull();
      // check Other properties here
    }

  }
}
