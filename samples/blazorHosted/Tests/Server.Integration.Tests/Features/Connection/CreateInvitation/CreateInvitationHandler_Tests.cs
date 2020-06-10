namespace CreateInvitationHandler
{
  using System.Threading.Tasks;
  using System.Text.Json;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using FluentAssertions;

  public class Handle_Returns : BaseTest
  {
    private readonly CreateInvitationRequest CreateInvitationRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      CreateInvitationRequest = new CreateInvitationRequest { Days = 10 };
    }

    public async Task CreateInvitationResponse()
    {
      CreateInvitationResponse CreateInvitationResponse = await Send(CreateInvitationRequest);

      ValidateCreateInvitationResponse(CreateInvitationResponse);
    }

    private void ValidateCreateInvitationResponse(CreateInvitationResponse aCreateInvitationResponse)
    {
      aCreateInvitationResponse.RequestId.Should().Be(CreateInvitationRequest.Id);
      // check Other properties here
    }

  }
}