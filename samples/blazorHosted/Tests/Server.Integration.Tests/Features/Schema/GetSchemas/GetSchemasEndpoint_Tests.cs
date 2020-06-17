﻿namespace GetSchemasEndpoint
{
  using FluentAssertions;
  using Microsoft.AspNetCore.Mvc.Testing;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using BlazorHosted.Features.Schemas;
  using BlazorHosted.Server.Integration.Tests.Infrastructure;
  using BlazorHosted.Server;

  public class Returns : BaseTest
  {
    private readonly GetSchemasRequest GetSchemasRequest;

    public Returns
    (
      WebApplicationFactory<Startup> aWebApplicationFactory,
      JsonSerializerOptions aJsonSerializerOptions
    ) : base(aWebApplicationFactory, aJsonSerializerOptions)
    {
      GetSchemasRequest = new GetSchemasRequest { Days = 10 };
    }

    public async Task GetSchemasResponse()
    {
      GetSchemasResponse GetSchemasResponse =
        await GetJsonAsync<GetSchemasResponse>(GetSchemasRequest.RouteFactory);

      ValidateGetSchemasResponse(GetSchemasResponse);
    }

    public async Task ValidationError()
    {
      // Set invalid value
      GetSchemasRequest.Days = -1;

      HttpResponseMessage httpResponseMessage = await HttpClient.GetAsync(GetSchemasRequest.RouteFactory);

      string json = await httpResponseMessage.Content.ReadAsStringAsync();

      httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
      json.Should().Contain("errors");
      json.Should().Contain(nameof(GetSchemasRequest.Days));
    }

    private void ValidateGetSchemasResponse(GetSchemasResponse aGetSchemasResponse)
    {
      aGetSchemasResponse.CorrelationId.Should().Be(GetSchemasRequest.CorrelationId);
      // check Other properties here
    }
  }
}