//namespace GetHealthHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.OpenApi.Features.Healths;
//  using Hyperledger.Aries.OpenApi.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetHealthRequest GetHealthRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetHealthRequest = new GetHealthRequest { Days = 10 };
//    }

//    public async Task GetHealthResponse()
//    {
//      GetHealthResponse GetHealthResponse = await Send(GetHealthRequest);

//      ValidateGetHealthResponse(GetHealthResponse);
//    }

//    private void ValidateGetHealthResponse(GetHealthResponse aGetHealthResponse)
//    {
//      aGetHealthResponse.CorrelationId.Should().Be(GetHealthRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}