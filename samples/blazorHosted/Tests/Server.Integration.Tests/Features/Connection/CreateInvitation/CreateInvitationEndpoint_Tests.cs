namespace CreateInvitationEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Connections;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Server;
  using System.Text;
  using System.Net.Mime;

  public class Returns : BaseTest
  {
    private readonly CreateInvitationRequest CreateInvitationRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      CreateInvitationRequest = new CreateInvitationRequest { Days = 10 };
    }

    public async Task CreateInvitationResponse()
    {
      CreateInvitationResponse createInvitationResponse =
        await Post(CreateInvitationRequest.RouteFactory, CreateInvitationRequest);

      ValidateCreateInvitationResponse(createInvitationResponse);
    }

    public async Task ValidationError()
    {
      // Set invalid value
      CreateInvitationRequest.Days = -1;
      //string requestAsJson = JsonSerializer.Serialize(CreateInvitationRequest, JsonSerializerOptions);
      //var content = 
      //  new StringContent
      //  (
      //    requestAsJson,
      //    Encoding.UTF8, 
      //    MediaTypeNames.Application.Json
      //  );

      //HttpResponseMessage httpResponseMessage = 
      //  await HttpClient.PostAsync(CreateInvitationRequest.RouteFactory, content);

      HttpResponseMessage httpResponseMessage = await GetHttpResponseMessageFromPost(CreateInvitationRequest.RouteFactory, CreateInvitationRequest);

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      json.Should().Contain("errors");
      json.Should().Contain(nameof(CreateInvitationRequest.Days));
    }

    private void ValidateCreateInvitationResponse(CreateInvitationResponse aCreateInvitationResponse)
    {
      aCreateInvitationResponse.RequestId.Should().Be(CreateInvitationRequest.RequestId);
      // check Other properties here
    }
  }
}