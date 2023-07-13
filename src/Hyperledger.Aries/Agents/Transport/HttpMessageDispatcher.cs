using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// Http message dispatcher.
    /// </summary>
    public class HttpMessageDispatcher : IMessageDispatcher
    {
        /// <summary>The HTTP client</summary>
        protected readonly HttpClient HttpClient;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public HttpMessageDispatcher(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory.CreateClient();
        }

        /// <inheritdoc />
        public string[] TransportSchemes => new[] { "http", "https" };

        /// <inheritdoc />
        public async Task<PackedMessageContext> DispatchAsync(Uri endpointUri, PackedMessageContext message)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = endpointUri,
                Method = HttpMethod.Post,
                Content = new ByteArrayContent(message.Payload)
            };

            var agentContentType = new MediaTypeHeaderValue(DefaultMessageService.AgentWireMessageMimeType);
            var encryptedEnvelopeContentType = new MediaTypeHeaderValue(DefaultMessageService.EncryptedEnvelopeMessageMimeType);
            request.Content.Headers.ContentType = encryptedEnvelopeContentType;

            var response = await HttpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new AriesFrameworkException(
                    ErrorCode.A2AMessageTransmissionError, $"Dispatch Failure. Endpoint:{endpointUri} Status: {response.StatusCode} Content: {responseBody}");
            }

            var responseContentType = response.Content?.Headers.ContentType; 
            if ((responseContentType?.Equals(encryptedEnvelopeContentType) ?? false)
                || (responseContentType?.Equals(agentContentType) ?? false))
            {
                var rawContent = await response.Content.ReadAsByteArrayAsync();

                //TODO this assumes all messages are packed
                if (rawContent.Length > 0)
                {
                    return new PackedMessageContext(rawContent);
                }
            }

            return null;
        }
    }
}
