namespace CreateInvitationHandler
{
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
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
