namespace CreateInvitationHandler
{
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System.Threading.Tasks;

  public class Handle_Returns : BaseTest
  {
    private readonly CreateInvitationRequest CreateInvitationRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      CreateInvitationRequest = CreateValidCreateInvitationRequest();
    }

    public async Task CreateInvitationResponse()
    {
      CreateInvitationResponse createInvitationResponse = await Send(CreateInvitationRequest);

      ValidateCreateInvitationResponse(CreateInvitationRequest, createInvitationResponse);
    }
  }
}
