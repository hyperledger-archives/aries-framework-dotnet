namespace GetConnectionEndpoint
{
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Newtonsoft.Json;
  using System;
  using System.Net.Http;
  using System.Net.Http.Json;
  using System.Threading.Tasks;

  public class Returns : BaseTest
  {
    private CreateInvitationResponse CreateInvitationResponse { get; set; }
    private GetConnectionRequest GetConnectionRequest { get; set; }

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetConnectionRequest = CreateValidGetConnectionRequest();
    }

    public async Task GetConnectionResponse_using_Json_Net()
    {
      GetConnectionRequest.ConnectionId = CreateInvitationResponse.ConnectionRecord.Id;

      GetConnectionResponse getConnectionResponse =
        await GetJsonAsync<GetConnectionResponse>(GetConnectionRequest.GetRoute());

      ValidateGetConnectionResponse(GetConnectionRequest, getConnectionResponse);
    }

    public async Task GetConnectionResponse_using_System_Text_Json()
    {
      GetConnectionRequest.ConnectionId = CreateInvitationResponse.ConnectionRecord.Id;

      GetConnectionResponse getConnectionResponse =
        await HttpClient.GetFromJsonAsync<GetConnectionResponse>(GetConnectionRequest.GetRoute());

      ValidateGetConnectionResponse(GetConnectionRequest, getConnectionResponse);
    }

    public async Task Setup()
    {
      await ResetAgent();
      CreateInvitationResponse = await CreateAnInvitation();
    }

    public void ValidationError()
    {
      // Arrange Set invalid value
      GetConnectionRequest.ConnectionId = null;

      // Act & Assert
      GetConnectionRequest.Invoking(aGetConnectionRequest => aGetConnectionRequest.GetRoute())
        .Should().Throw<ArgumentNullException>();
    }
  }
}
