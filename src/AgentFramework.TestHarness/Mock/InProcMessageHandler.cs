using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Runtime;

namespace AgentFramework.TestHarness.Mock
{
    public class InProcMessageHandler : HttpMessageHandler
    {
        public InProcAgent TargetAgent { get; set; }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await TargetAgent.HandleAsync(await request.Content.ReadAsByteArrayAsync());
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            if (response != null)
            {
                responseMessage.Content = new ByteArrayContent(response.GetData());
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(DefaultMessageService.AgentWireMessageMimeType);
            }

            return responseMessage;
        }
    }
}