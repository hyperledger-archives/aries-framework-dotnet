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
    public class AgentMiddleware : IMiddleware
    {
        private readonly IAgentProvider _contextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentMiddleware"/> class.
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
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!HttpMethods.IsPost(context.Request.Method)
                || !context.Request.ContentType.Equals(DefaultMessageService.AgentWireMessageMimeType))
            {
                await next(context);
                return;
            }

            if (context.Request.ContentLength == null) throw new Exception("Empty content length");

            using (var stream = new StreamReader(context.Request.Body))
            {
                var body = await stream.ReadToEndAsync();

                var agent = await _contextProvider.GetAgentAsync();
                var response = await agent.ProcessAsync(
                    context: await _contextProvider.GetContextAsync(), //TODO assumes all received messages are packed 
                    messageContext: new PackedMessageContext(body.GetUTF8Bytes()));

                context.Response.StatusCode = 200;

                if (response != null)
                {
                    context.Response.ContentType = DefaultMessageService.AgentWireMessageMimeType;
					await context.Response.WriteAsync(response.Payload.GetUTF8String());
				}
                else
                    await context.Response.WriteAsync(string.Empty);
            }
        }
    }
}