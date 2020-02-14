using System;
using System.IO;
using System.Threading.Tasks;
using Hyperledger.Aries.Extensions;
using Microsoft.AspNetCore.Http;
using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.AspNetCore
{
    /// <summary>
    /// An agent middleware
    /// </summary>
    public class AgentMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentMiddleware"/> class.
        /// </summary>
        /// <param name="next">Context provider.</param>
        public AgentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the agent processing pipeline
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="agentProvider"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext, IAgentProvider agentProvider)
        {
            if (!HttpMethods.IsPost(httpContext.Request.Method)
                || !(httpContext.Request.ContentType?.Equals(DefaultMessageService.AgentWireMessageMimeType) ?? false))
            {
                await _next(httpContext);
                return;
            }

            if (httpContext.Request.ContentLength == null) throw new Exception("Empty content length");

            using var stream = new StreamReader(httpContext.Request.Body);
            var body = await stream.ReadToEndAsync();

            var agent = await agentProvider.GetAgentAsync();

            var response = await agent.ProcessAsync(
                context: await agentProvider.GetContextAsync(), //TODO assumes all received messages are packed 
                messageContext: new PackedMessageContext(body.GetUTF8Bytes()));

            httpContext.Response.StatusCode = 200;

            if (response != null)
            {
                httpContext.Response.ContentType = DefaultMessageService.AgentWireMessageMimeType;
                await httpContext.Response.WriteAsync(response.Payload.GetUTF8String());
            }
            else
                await httpContext.Response.WriteAsync(string.Empty);
        }
    }
}
