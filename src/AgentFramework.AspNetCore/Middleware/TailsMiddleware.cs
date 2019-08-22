using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AgentFramework.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace AgentFramework.AspNetCore.Middleware
{
    /// <summary>
    /// A middleware that serves tails data for revocation registries
    /// </summary>
    public class TailsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="TailsMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next.</param>
        public TailsMiddleware(RequestDelegate next)
        {
            _next = next;
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
                data = File.ReadAllBytes(
                    Path.Combine(
                        EnvironmentUtils.GetTailsPath(), uri.Segments.Last()));
            }
            catch (Exception ex) when (
                ex is FileNotFoundException
                || ex is DirectoryNotFoundException)
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }

            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.ContentType = "application/octet-stream";
            await context.Response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}