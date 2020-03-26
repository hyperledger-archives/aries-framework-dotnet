using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.AspNetCore
{
    /// <summary>
    /// A middleware that serves tails data for revocation registries
    /// </summary>
    public class TailsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TailsMiddleware> _logger;
        private readonly AgentOptions _agentOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TailsMiddleware" /> class.
        /// </summary>
        /// <param name="next">The next.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="agentOptions">The agent options.</param>
        public TailsMiddleware(
            RequestDelegate next,
            ILogger<TailsMiddleware> logger,
            IOptions<AgentOptions> agentOptions)
        {
            _next = next;
            _logger = logger;
            _agentOptions = agentOptions.Value;
        }

        /// <summary>
        /// Middleware invocation method
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            byte[] data;
            try
            {
                var uri = new Uri(context.Request.GetEncodedUrl());
                data = File.ReadAllBytes(Path.Combine(_agentOptions.RevocationRegistryDirectory, uri.Segments.Last()));
            }
            catch (Exception ex) when (
                ex is FileNotFoundException
                || ex is DirectoryNotFoundException)
            {
                _logger.LogWarning("Can't locate tails file");
                await _next(context);
                return;
            }

            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.ContentType = "application/octet-stream";
            await context.Response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}