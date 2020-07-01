//namespace ResetWalletEndpoint
//{
//  using FluentAssertions;
//  using Microsoft.AspNetCore.Mvc.Testing;
//  using System.Net;
//  using System.Net.Http;
//  using System.Threading.Tasks;
//  using BlazorHosted.Features.Wallets;
//  using BlazorHosted.Server.Integration.Tests.Infrastructure;
//  using BlazorHosted.Server;
//  using Newtonsoft.Json;

//  public class Returns : BaseTest
//  {
//    private readonly ResetWalletRequest ResetWalletRequest;

//    public Returns
//    (
//      WebApplicationFactory<Startup> aWebApplicationFactory,
//      JsonSerializerSettings aJsonSerializerSettings
//    ) : base(aWebApplicationFactory, aJsonSerializerSettings)
//    {
//      ResetWalletRequest = new ResetWalletRequest { SampleProperty = 10 };
//    }

//    public async Task ResetWalletResponse()
//    {
//      ResetWalletResponse ResetWalletResponse =
//        await GetJsonAsync<ResetWalletResponse>(ResetWalletRequest.GetRoute());

//      ValidateResetWalletResponse(ResetWalletResponse);
//    }

//    public async Task ValidationError()
//    {
//      // Set invalid value
//      //ResetWalletRequest.SampleProperty = -1;

//      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(ResetWalletRequest.GetRoute());

//      string json = await httpResponseMessage.Content.ReadAsStringAsync();

//      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
//      json.Should().Contain("errors");
//      json.Should().Contain(nameof(ResetWalletRequest.SampleProperty));
//    }

//    private void ValidateResetWalletResponse(ResetWalletResponse aResetWalletResponse)
//    {
//      aResetWalletResponse.CorrelationId.Should().Be(ResetWalletRequest.CorrelationId);
//      // check Other properties here
//    }
//  }
//}