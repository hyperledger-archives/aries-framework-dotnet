//namespace GetProofsHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Features.PresentProofs;
//  using Hyperledger.Aries.AspNetCore.Server;
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