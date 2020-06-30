namespace CreateInvitationHandler
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc.Testing;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using Newtonsoft.Json;
  using TestHelpers;

  public class Handle_Returns : BaseTest
  {
    private readonly CreateInvitationRequest CreateInvitationRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      CreateInvitationRequest = CreateInvitationTestHelper.CreateValidCreateInvitationRequest();
    }

    public async Task CreateInvitationResponse()
    {
      CreateInvitationResponse createInvitationResponse = await Send(CreateInvitationRequest);

      CreateInvitationTestHelper.ValidateCreateInvitationResponse(CreateInvitationRequest, createInvitationResponse);
    }
  }
}