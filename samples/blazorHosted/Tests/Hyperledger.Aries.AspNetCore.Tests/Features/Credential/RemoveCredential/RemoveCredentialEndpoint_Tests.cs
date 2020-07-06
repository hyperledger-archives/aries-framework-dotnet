//namespace RemoveCredentialEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Text.Json;
//  using System.Threading.Tasks;
//  using Hyperledger.Aries.AspNetCore.Features.Credentials;
//  using Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure;
//  using Hyperledger.Aries.AspNetCore.Server;

//  public class Returns : BaseTest
//  {
//    private readonly RemoveCredentialRequest RemoveCredentialRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerOptions aJsonSerializerOptions
//    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
//    {
//      RemoveCredentialRequest = new RemoveCredentialRequest { Days = 10 };
//    }

//    public async Task RemoveCredentialResponse()
//    {
//      RemoveCredentialResponse RemoveCredentialResponse =
//        await GetJsonAsync<RemoveCredentialResponse>(RemoveCredentialRequest.GetRoute());

//      ValidateRemoveCredentialResponse(RemoveCredentialResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      RemoveCredentialRequest.Days = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(RemoveCredentialRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(RemoveCredentialRequest.Days));
//    }

//    private void ValidateRemoveCredentialResponse(RemoveCredentialResponse aRemoveCredentialResponse)
//    {
//      aRemoveCredentialResponse.CorrelationId.Should().Be(RemoveCredentialRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}