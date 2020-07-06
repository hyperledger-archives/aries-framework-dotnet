//namespace GetSchemaHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Features.Schemas;
//  using Hyperledger.Aries.AspNetCore.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetSchemaRequest GetSchemaRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetSchemaRequest = new GetSchemaRequest { Days = 10 };
//    }

//    public async Task GetSchemaResponse()
//    {
//      GetSchemaResponse GetSchemaResponse = await Send(GetSchemaRequest);

//      ValidateGetSchemaResponse(GetSchemaResponse);
//    }

//    private void ValidateGetSchemaResponse(GetSchemaResponse aGetSchemaResponse)
//    {
//      aGetSchemaResponse.CorrelationId.Should().Be(GetSchemaRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}