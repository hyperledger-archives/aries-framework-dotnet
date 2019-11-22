using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;

namespace Hyperledger.TestHarness.Mock
{
    public class MockAgentHttpHandler : HttpMessageHandler
    {
        public MockAgentHttpHandler(Func<(string name, byte[] data), Task<MessageContext>> callback)
        {
            Callback = callback;
        }

        public Func<(string, byte[]), Task<MessageContext>> Callback { get; }

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
                responseMessage.Content = new ByteArrayContent(response.Payload);
                responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(DefaultMessageService.AgentWireMessageMimeType);
            }

            return responseMessage;
        }
    }
}
