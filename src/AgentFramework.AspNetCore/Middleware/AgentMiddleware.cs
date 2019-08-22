using System;
using System.IO;
using System.Threading.Tasks;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Runtime;
using Microsoft.AspNetCore.Http;

namespace AgentFramework.AspNetCore.Middleware
{
    /// <summary>
    /// An agent middleware
    /// </summary>
    public class AgentMiddleware : IMiddleware
    {
        private readonly IAgentProvider _contextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.AspNetCore.Middleware.AgentMiddleware"/> class.
        /// </summary>
        /// <param name="contextProvider">Context provider.</param>
        public AgentMiddleware(IAgentProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        /// <summary>
        /// Invokes the agent processing pipeline
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <param name="contextProvider"></param>
        /// <returns></returns>
        public static async Task InvokeAsync(HttpContext context, Func<Task> next, IAgentProvider contextProvider)
        {
            if (!HttpMethods.IsPost(context.Request.Method)
                || !context.Request.ContentType.Equals(DefaultMessageService.AgentWireMessageMimeType))
            {
                await next();
                return;
            }

            if (context.Request.ContentLength == null) throw new Exception("Empty content length");

            using (var stream = new StreamReader(context.Request.Body))
            {
                var body = await stream.ReadToEndAsync();

                var agent = await contextProvider.GetAgentAsync();
                var response = await agent.ProcessAsync(
                    context: await contextProvider.GetContextAsync(), //TODO assumes all received messages are packed 
                    messageContext: new MessageContext(body.GetUTF8Bytes(), true));

                context.Response.StatusCode = 200;

                if (response != null)
                {
                    context.Response.ContentType = DefaultMessageService.AgentWireMessageMimeType;
                    await response.Stream.CopyToAsync(context.Response.Body);
                }
                else
                    await context.Response.WriteAsync(string.Empty);
            }
        }

        /// <inheritdoc />
        public Task InvokeAsync(HttpContext context, RequestDelegate next) =>
            InvokeAsync(context, async () => await next(context), _contextProvider);
    }
}