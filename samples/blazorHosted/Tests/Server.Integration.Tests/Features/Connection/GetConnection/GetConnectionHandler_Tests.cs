//namespace GetConnectionHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using BlazorHosted.Server.Integration.Tests.Infrastructure;
//  using BlazorHosted.Features.Connections;
//  using BlazorHosted.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetConnectionRequest GetConnectionRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetConnectionRequest = new GetConnectionRequest { Days = 10 };
//    }

//    public async Task GetConnectionResponse()
//    {
//      GetConnectionResponse GetConnectionResponse = await Send(GetConnectionRequest);

//      ValidateGetConnectionResponse(GetConnectionResponse);
//    }

//    private void ValidateGetConnectionResponse(GetConnectionResponse aGetConnectionResponse)
//    {
//      aGetConnectionResponse.CorrelationId.Should().Be(GetConnectionRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}