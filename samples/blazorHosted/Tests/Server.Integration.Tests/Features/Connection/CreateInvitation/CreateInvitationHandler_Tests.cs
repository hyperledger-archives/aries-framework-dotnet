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
      CreateInvitationRequest = new CreateInvitationRequest { Alias = "Alice" };
    }

    public async Task CreateInvitationResponse()
    {
      CreateInvitationResponse CreateInvitationResponse = await Send(CreateInvitationRequest);

      ValidateCreateInvitationResponse(CreateInvitationResponse);
    }

    private void ValidateCreateInvitationResponse(CreateInvitationResponse aCreateInvitationResponse)
    {
      aCreateInvitationResponse.CorrelationId.Should().Be(CreateInvitationRequest.CorrelationId);
      // check Other properties here
    }

  }
}