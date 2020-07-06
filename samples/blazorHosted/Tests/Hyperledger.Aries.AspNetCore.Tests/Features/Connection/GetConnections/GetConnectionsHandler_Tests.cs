namespace GetConnectionsHandler
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server;
  using Newtonsoft.Json;

  public class Handle_Returns : BaseTest
  {
    private readonly GetConnectionsRequest GetConnectionsRequest;

    public Handle_Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetConnectionsRequest = CreateValidGetConnectionsRequest();
    }

    public async Task GetConnectionsResponse()
    {
      // Arrage
      await CreateAnInvitation();

      // Act
      GetConnectionsResponse getConnectionsResponse = await Send(GetConnectionsRequest);

      // Assert
      ValidateGetConnectionsResponse(GetConnectionsRequest, getConnectionsResponse);
    }

    public async Task Setup() => await ResetAgent();
  }
}