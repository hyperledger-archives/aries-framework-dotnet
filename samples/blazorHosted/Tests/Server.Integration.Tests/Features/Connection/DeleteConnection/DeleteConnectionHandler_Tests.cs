//namespace DeleteConnectionHandler
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
//    private readonly DeleteConnectionRequest DeleteConnectionRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      DeleteConnectionRequest = new DeleteConnectionRequest { Days = 10 };
//    }

//    public async Task DeleteConnectionResponse()
//    {
//      DeleteConnectionResponse DeleteConnectionResponse = await Send(DeleteConnectionRequest);

//      ValidateDeleteConnectionResponse(DeleteConnectionResponse);
//    }

//    private void ValidateDeleteConnectionResponse(DeleteConnectionResponse aDeleteConnectionResponse)
//    {
//      aDeleteConnectionResponse.CorrelationId.Should().Be(DeleteConnectionRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}