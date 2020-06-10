namespace BlazorHosted.Server.Integration.Tests.Infrastructure
{
  using BlazorHosted.Server;
  using MediatR;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Net.Http;
  using System.Net.Mime;
  using System.Text;
  using System.Text.Json;
  using System.Threading.Tasks;

  /// <summary>
  ///
  /// </summary>
  /// <remarks>
  /// Based on Jimmy's SliceFixture
  /// https://github.com/jbogard/ContosoUniversityDotNetCore-Pages/blob/master/ContosoUniversity.IntegrationTests/SliceFixture.cs
  /// </remarks>
  public abstract class BaseTest
  {
    public readonly JsonSerializerOptions JsonSerializerOptions;
    protected readonly HttpClient HttpClient;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public BaseTest(WebApplicationFactory<Startup> aWebApplicationFactory, JsonSerializerOptions aJsonSerializerOptions)
    {
      ServiceScopeFactory = aWebApplicationFactory.Services.GetService<IServiceScopeFactory>();
      HttpClient = aWebApplicationFactory.CreateClient();
      JsonSerializerOptions = aJsonSerializerOptions;
    }

    protected async Task<T> ExecuteInScope<T>(Func<IServiceProvider, Task<T>> aAction)
    {
      using IServiceScope serviceScope = ServiceScopeFactory.CreateScope();
      return await aAction(serviceScope.ServiceProvider);
    }

    protected async Task<HttpResponseMessage> GetHttpResponseMessageFromPost<TResponse>(string aUri, IRequest<TResponse> aRequest)
    {
      string requestAsJson = JsonSerializer.Serialize(aRequest, aRequest.GetType(), JsonSerializerOptions);

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

      httpResponseMessage.EnsureSuccessStatusCode();

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      TResponse response = JsonSerializer.Deserialize<TResponse>(json, JsonSerializerOptions);

      return response;
    }

    protected async Task<TResponse> Post<TResponse>(string aUri, IRequest<TResponse> aRequest)
    {
      HttpResponseMessage httpResponseMessage = await GetHttpResponseMessageFromPost(aUri, aRequest);

      httpResponseMessage.EnsureSuccessStatusCode();

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      TResponse response = JsonSerializer.Deserialize<TResponse>(json, JsonSerializerOptions);

      return response;
    }

    protected Task Send(IRequest aRequest)
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

    protected Task<TResponse> Send<TResponse>(IRequest<TResponse> aRequest)
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
  }
}
