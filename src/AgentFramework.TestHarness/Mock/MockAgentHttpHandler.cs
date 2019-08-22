using AgentFramework.Core.Handlers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Runtime;

namespace AgentFramework.TestHarness.Mock
{
    public class MockAgentHttpHandler : HttpMessageHandler
    {
        public MockAgentHttpHandler(Func<(string name, byte[] data), Task<MessageResponse>> callback)
        {
            Callback = callback;
        }

        public Func<(string, byte[]), Task<MessageResponse>> Callback { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Post)
            {
                throw new Exception("Invalid http method");
            }

            var response = await Callback((request.RequestUri.Host, await request.Content.ReadAsByteArrayAsync()));
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            if (response != null)
            {
                responseMessage.Content = new ByteArrayContent(response.GetData());
                responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(DefaultMessageService.AgentWireMessageMimeType);
            }

            return responseMessage;
        }
    }
}
