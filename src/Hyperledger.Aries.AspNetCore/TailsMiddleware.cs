namespace Hyperledger.Aries.AspNetCore
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using Hyperledger.Aries.Configuration;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Http.Extensions;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;

  /// <summary>
  /// A middleware that serves tails data for revocation registries
  /// </summary>
  public class TailsMiddleware
  {
    private readonly RequestDelegate NextRequestDelegate;
    private readonly ILogger<TailsMiddleware> Logger;
    private readonly AgentOptions AgentOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TailsMiddleware" /> class.
    /// </summary>
    /// <param name="aNextRequestDelegate">The next.</param>
    /// <param name="aLogger">The logger.</param>
    /// <param name="aAgentOptions">The agent options.</param>
    public TailsMiddleware
    (
      RequestDelegate aNextRequestDelegate,
      ILogger<TailsMiddleware> aLogger,
      IOptions<AgentOptions> aAgentOptions
    )
    {
      NextRequestDelegate = aNextRequestDelegate;
      Logger = aLogger;
      AgentOptions = aAgentOptions.Value;
    }

    /// <summary>
    /// Middleware invocation method
    /// </summary>
    /// <param name="aHttpContext">The http context.</param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext aHttpContext)
    {
      if (!aHttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
      {
        await NextRequestDelegate(aHttpContext);
        return;
      }

      byte[] data;
      try
      {
        var uri = new Uri(aHttpContext.Request.GetEncodedUrl());
        data = File.ReadAllBytes(Path.Combine(AgentOptions.RevocationRegistryDirectory, uri.Segments.Last()));
      }
      catch (Exception ex) when (
          ex is FileNotFoundException
          || ex is DirectoryNotFoundException)
      {
        Logger.LogWarning("Can't locate tails file");
        await NextRequestDelegate(aHttpContext);
        return;
      }

      aHttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
      aHttpContext.Response.ContentType = "application/octet-stream";
      await aHttpContext.Response.Body.WriteAsync(data, 0, data.Length);
    }
  }
}