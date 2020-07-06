namespace Hyperledger.Aries.AspNetCore.Server.Integration.Tests.Infrastructure
{
  using Hyperledger.Aries.AspNetCore.Server;
  using FluentAssertions;
  using MediatR;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Microsoft.Extensions.DependencyInjection;
  using Newtonsoft.Json;
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Net.Mime;
  using System.Text;
  using System.Threading.Tasks;

  /// <summary>
  /// BaseTest case
  /// </summary>
  /// <remarks>
  /// Based on Jimmy's SliceFixture
  /// https://github.com/jbogard/ContosoUniversityDotNetCore-Pages/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
  /// </remarks>
  public abstract partial class BaseTest
  {
    public readonly JsonSerializerSettings JsonSerializerSettings;
    protected readonly HttpClient HttpClient;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public BaseTest(WebApplicationFactory<Startup> aWebApplicationFactory, JsonSerializerSettings aJsonSerializerSettings)
    {
      ServiceScopeFactory = aWebApplicationFactory.Services.GetService<IServiceScopeFactory>();
      HttpClient = aWebApplicationFactory.CreateClient();
      JsonSerializerSettings = aJsonSerializerSettings;
    }

    internal Task Send(IRequest aRequest)
    {
      return ExecuteInScope
      (
        aServiceProvider =>
        {
          IMediator mediator = aServiceProvider.GetService<IMediator>();

          return mediator.Send(aRequest);
        }
      );
    }

    internal Task<TResponse> Send<TResponse>(IRequest<TResponse> aRequest)
    {
      return ExecuteInScope
      (
        aServiceProvider =>
        {
          IMediator mediator = aServiceProvider.GetService<IMediator>();

          return mediator.Send(aRequest);
        }
      );
    }

    protected async Task ConfirmEndpointValidationError<TResponse>(string aRoute, IRequest<TResponse> aRequest, string aAttributeName)
    {
      HttpResponseMessage httpResponseMessage = await GetHttpResponseMessageFromPost(aRoute, aRequest);
      await ConfirmEndpointValidationError(httpResponseMessage, aAttributeName);
    }

    protected async Task ConfirmEndpointValidationError(HttpResponseMessage aHttpResponseMessage, string aAttributeName)
    {
      string json = await aHttpResponseMessage.Content.ReadAsStringAsync();

      aHttpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      json.Should().Contain("errors");
      json.Should().Contain(aAttributeName);
    }

    protected async Task<T> ExecuteInScope<T>(Func<IServiceProvider, Task<T>> aAction)
    {
      using IServiceScope serviceScope = ServiceScopeFactory.CreateScope();
      return await aAction(serviceScope.ServiceProvider);
    }

    protected async Task<HttpResponseMessage> GetHttpResponseMessageFromPost<TResponse>(string aUri, IRequest<TResponse> aRequest)
    {
      string requestAsJson = JsonConvert.SerializeObject(aRequest, aRequest.GetType(), JsonSerializerSettings);

      var httpContent =
        new StringContent
        (
          requestAsJson,
          Encoding.UTF8,
          MediaTypeNames.Application.Json
        );

      HttpResponseMessage httpResponseMessage = await HttpClient.PostAsync(aUri, httpContent);
      return httpResponseMessage;
    }

    protected async Task<TResponse> GetJsonAsync<TResponse>(string aUri)
    {
      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(aUri);

      return await ReadFromJson<TResponse>(httpResponseMessage);
    }

    protected async Task<TResponse> DeleteJsonAsync<TResponse>(string aUri)
    {
      HttpResponseMessage httpResponseMessage = await HttpClient.DeleteAsync(aUri);
      return await ReadFromJson<TResponse>(httpResponseMessage);
    }

    private async Task<TResponse> ReadFromJson<TResponse>(HttpResponseMessage aHttpResponseMessage)
    {
      aHttpResponseMessage.EnsureSuccessStatusCode();

      string json = await aHttpResponseMessage.Content.ReadAsStringAsync();

      TResponse response = JsonConvert.DeserializeObject<TResponse>(json, JsonSerializerSettings);

      return response;
    }

    protected async Task<TResponse> Post<TResponse>(string aUri, IRequest<TResponse> aRequest)
    {
      HttpResponseMessage httpResponseMessage = await GetHttpResponseMessageFromPost(aUri, aRequest);

      httpResponseMessage.EnsureSuccessStatusCode();

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      TResponse response = JsonConvert.DeserializeObject<TResponse>(json, JsonSerializerSettings);

      return response;
    }
  }
}
