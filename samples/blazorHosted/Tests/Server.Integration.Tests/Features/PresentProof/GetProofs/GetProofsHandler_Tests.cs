//namespace GetProofsHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.OpenApi.Features.PresentProofs;
//  using Hyperledger.Aries.OpenApi.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly GetProofsRequest GetProofsRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetProofsRequest = new GetProofsRequest { Days = 10 };
//    }

//    public async Task GetProofsResponse()
//    {
//      GetProofsResponse GetProofsResponse = await Send(GetProofsRequest);

//      ValidateGetProofsResponse(GetProofsResponse);
//    }

//    private void ValidateGetProofsResponse(GetProofsResponse aGetProofsResponse)
//    {
//      aGetProofsResponse.CorrelationId.Should().Be(GetProofsRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}