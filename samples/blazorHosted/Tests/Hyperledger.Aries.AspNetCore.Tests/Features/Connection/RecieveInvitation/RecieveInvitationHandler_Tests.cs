namespace ReceiveInvitationHandler
{
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using FluentAssertions;
  using Newtonsoft.Json;
  using Hyperledger.Aries.Features.DidExchange;

  public class Handle_Returns : BaseTest
  {
    private readonly ReceiveInvitationRequest ReceiveInvitationRequest;

    public Handle_Returns
    (
      AliceWebApplicationFactory aAliceWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aAliceWebApplicationFactory, aJsonSerializerSettings)
    {
      ReceiveInvitationRequest = CreateValidReceiveInvitationRequest();
    }

    public async Task ReceiveInvitationResponse()
    {
      ReceiveInvitationResponse receiveInvitationResponse = await Send(ReceiveInvitationRequest);

      ValidateReceiveInvitationResponse(ReceiveInvitationRequest, receiveInvitationResponse);
    }

    public async Task Setup()
    {
      await ResetAgent();
    }
  }
}