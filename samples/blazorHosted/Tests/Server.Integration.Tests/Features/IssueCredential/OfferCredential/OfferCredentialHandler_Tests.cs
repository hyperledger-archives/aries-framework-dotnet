//namespace OfferCredentialHandler
//{
//  using System.Threading.Tasks;
//  using System.Text.Json;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using Hyperledger.Aries.OpenApi.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.OpenApi.Features.IssueCredentials;
//  using Hyperledger.Aries.OpenApi.Server;
//  using FluentAssertions;

//  public class Handle_Returns : BaseTest
//  {
//    private readonly OfferCredentialRequest OfferCredentialRequest;

//    public Handle_Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      OfferCredentialRequest = new OfferCredentialRequest { Days = 10 };
//    }

//    public async Task OfferCredentialResponse()
//    {
//      OfferCredentialResponse OfferCredentialResponse = await Send(OfferCredentialRequest);

//      ValidateOfferCredentialResponse(OfferCredentialResponse);
//    }

//    private void ValidateOfferCredentialResponse(OfferCredentialResponse aOfferCredentialResponse)
//    {
//      aOfferCredentialResponse.CorrelationId.Should().Be(OfferCredentialRequest.CorrelationId);
//      // check Other properties here
//    }

//  }
//}