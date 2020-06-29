//namespace DeleteConnectionEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using BlazorHosted.Features.Connections;
//  using BlazorHosted.Server.Integration.Tests.Infrastructure;
//  using BlazorHosted.Server;

//  public class Returns : BaseTest
//  {
//    private readonly DeleteConnectionRequest DeleteConnectionRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      DeleteConnectionRequest = new DeleteConnectionRequest { Days = 10 };
//    }

//    public async Task DeleteConnectionResponse()
//    {
//      DeleteConnectionResponse DeleteConnectionResponse =
//        await GetJsonAsync<DeleteConnectionResponse>(DeleteConnectionRequest.GetRoute());

//      ValidateDeleteConnectionResponse(DeleteConnectionResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      DeleteConnectionRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(DeleteConnectionRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(DeleteConnectionRequest.Days));
//    }

//    private void ValidateDeleteConnectionResponse(DeleteConnectionResponse aDeleteConnectionResponse)
//    {
//      aDeleteConnectionResponse.CorrelationId.Should().Be(DeleteConnectionRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}