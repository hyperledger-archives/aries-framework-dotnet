namespace GetConnectionsEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using Hyperledger.Aries.AspNetCore.Features.Connections;
  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
  using Hyperledger.Aries.AspNetCore.Server;
  using Newtonsoft.Json;
  using System.Net.Http.Json;

  public class Returns : BaseTest
  {
    private readonly GetConnectionsRequest GetConnectionsRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerSettings aJsonSerializerSettings
    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
    {
      GetConnectionsRequest = new GetConnectionsRequest();
    }

    public async Task GetConnectionsResponse_using_Json_Net()
    {

      GetConnectionsResponse getConnectionsResponse =
        await GetJsonAsync<GetConnectionsResponse>(GetConnectionsRequest.GetRoute());

      ValidateGetConnectionsResponse(GetConnectionsRequest, getConnectionsResponse);
    }

    public async Task GetConnectionsResponse_using_System_Text_Json()
    {
      GetConnectionsResponse getConnectionsResponse =
        await HttpClient.GetFromJsonAsync<GetConnectionsResponse>(GetConnectionsRequest.GetRoute());

      ValidateGetConnectionsResponse(GetConnectionsRequest, getConnectionsResponse);
    }

    // There are no validation requirements for this request
    public void ValidationError() { }

    public async Task Setup()
    {
      await ResetAgent();
      await CreateAnInvitation();
    }
  }
}