//namespace OfferCredentialEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.IssueCredentials;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly OfferCredentialRequest OfferCredentialRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      OfferCredentialRequest = new OfferCredentialRequest { Days = 10 };
//    }

//    public async Task OfferCredentialResponse()
//    {
//      OfferCredentialResponse OfferCredentialResponse =
//        await GetJsonAsync<OfferCredentialResponse>(OfferCredentialRequest.GetRoute());

//      ValidateOfferCredentialResponse(OfferCredentialResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      OfferCredentialRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(OfferCredentialRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(OfferCredentialRequest.Days));
//    }

//    private void ValidateOfferCredentialResponse(OfferCredentialResponse aOfferCredentialResponse)
//    {
//      aOfferCredentialResponse.CorrelationId.Should().Be(OfferCredentialRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}