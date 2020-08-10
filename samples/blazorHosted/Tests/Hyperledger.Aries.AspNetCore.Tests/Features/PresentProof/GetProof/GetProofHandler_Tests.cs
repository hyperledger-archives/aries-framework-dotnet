//namespace GetProofHandler
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
//    private readonly GetProofRequest GetProofRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      GetProofRequest = new GetProofRequest { Days = 10 };
//    }

//    public async Task GetProofResponse()
//    {
//      GetProofResponse GetProofResponse = await Send(GetProofRequest);

//      ValidateGetProofResponse(GetProofResponse);
//    }

//    private void ValidateGetProofResponse(GetProofResponse aGetProofResponse)
//    {
//      aGetProofResponse.CorrelationId.Should().Be(GetProofRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}