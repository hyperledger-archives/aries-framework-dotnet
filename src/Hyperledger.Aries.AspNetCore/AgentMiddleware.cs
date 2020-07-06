namespace Hyperledger.Aries.AspNetCore
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Hyperledger.Aries.Extensions;
  using Microsoft.AspNetCore.Http;
  using Hyperledger.Aries.Agents;

  /// <summary>
  /// An agent middleware
  /// </summary>
  public class AgentMiddleware
  {
    private readonly RequestDelegate NextRequestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentMiddleware"/> class.
    /// </summary>
    /// <param name="aNext">Context provider.</param>
    public AgentMiddleware(RequestDelegate aNext)
    {
      NextRequestDelegate = aNext;
    }

    /// <summary>
    /// Invokes the agent processing pipeline
    /// </summary>
    /// <param name="aHttpContext"></param>
    /// <param name="aAgentProvider"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext aHttpContext, IAgentProvider aAgentProvider)
    {
      if
      (
        !HttpMethods.IsPost(aHttpContext.Request.Method) ||
        !(aHttpContext.Request.ContentType?.Equals(DefaultMessageService.AgentWireMessageMimeType) ?? false)
      )
      {
        await NextRequestDelegate(aHttpContext);
        return;
      }

      if (aHttpContext.Request.ContentLength == null) throw new Exception("Empty content length");

      using var stream = new StreamReader(aHttpContext.Request.Body);
      string body = await stream.ReadToEndAsync();

      IAgent agent = await aAgentProvider.GetAgentAsync();

      MessageContext response =
        await agent.ProcessAsync
        (
          context: await aAgentProvider.GetContextAsync(), //TODO assumes all received messages are packed 
          messageContext: new PackedMessageContext(body.GetUTF8Bytes())
        );

      aHttpContext.Response.StatusCode = 200;

      if (response != null)
      {
        aHttpContext.Response.ContentType = DefaultMessageService.AgentWireMessageMimeType;
        await aHttpContext.Response.WriteAsync(response.Payload.GetUTF8String());
      }
      else
        await aHttpContext.Response.WriteAsync(string.Empty);
    }
  }
}
