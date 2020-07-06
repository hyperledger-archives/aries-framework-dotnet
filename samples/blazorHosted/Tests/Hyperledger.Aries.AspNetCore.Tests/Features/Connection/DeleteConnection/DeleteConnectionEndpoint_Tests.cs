namespace DeleteConnectionEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Hyperledger.Aries.AspNetCore.Server;
  using Newtonsoft.Json;
  using System;
  using System.Net.Http.Json;

  public class Returns : BaseTest
  {
    private readonly DeleteConnectionRequest DeleteConnectionRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      DeleteConnectionRequest = new DeleteConnectionRequest("ConnectionId");
    }

    public async Task DeleteConnectionResponse_using_Json_Net()
    {
      DeleteConnectionResponse DeleteConnectionResponse =
        await DeleteJsonAsync<DeleteConnectionResponse>(DeleteConnectionRequest.GetRoute());

      ValidateDeleteConnectionResponse(DeleteConnectionRequest, DeleteConnectionResponse);
    }

    public async Task GetConnectionsResponse_using_System_Text_Json()
    {
      HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync(DeleteConnectionRequest.GetRoute());

      httpResponseMessage.EnsureSuccessStatusCode();

      DeleteConnectionResponse deleteConnectionResponse = 
        await httpResponseMessage.Content.ReadFromJsonAsync<DeleteConnectionResponse>();

      deleteConnectionResponse.Should().NotBeNull();
    }


    public void ValidationError()
    {
      // Arrange Set invalid value
      DeleteConnectionRequest.ConnectionId = null;

      // Act & Assert
      DeleteConnectionRequest.Invoking(aGetConnectionRequest => aGetConnectionRequest.GetRoute())
        .Should().Throw<ArgumentNullException>();

    }
  }
}