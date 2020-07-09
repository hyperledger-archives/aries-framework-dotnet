//namespace GetSchemasHandler
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
//    private readonly GetSchemasRequest GetSchemasRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetSchemasRequest = new GetSchemasRequest { Days = 10 };
//    }

//    public async Task GetSchemasResponse()
//    {
//      GetSchemasResponse GetSchemasResponse = await Send(GetSchemasRequest);

//      ValidateGetSchemasResponse(GetSchemasResponse);
//    }

//    private void ValidateGetSchemasResponse(GetSchemasResponse aGetSchemasResponse)
//    {
//      aGetSchemasResponse.CorrelationId.Should().Be(GetSchemasRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}