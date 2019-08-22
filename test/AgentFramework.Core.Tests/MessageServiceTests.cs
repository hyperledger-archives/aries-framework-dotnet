using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Connections;
using AgentFramework.Core.Messages.Routing;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Connections;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Runtime;
using AgentFramework.Core.Runtime.Transport;
using AgentFramework.Core.Utils;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AgentFramework.Core.Tests
{
    public class MockAgentMessage : AgentMessage { }

    public class MessageServiceTests : IAsyncLifetime
    {
        private string Config = "{\"id\":\"" + Guid.NewGuid() + "\"}";
        private const string WalletCredentials = "{\"key\":\"test_wallet_key\"}";

        private Wallet _wallet;

        private readonly IMessageService _messagingService;

        private readonly ConcurrentBag<HttpRequestMessage> _messages = new ConcurrentBag<HttpRequestMessage>();

        public MessageServiceTests()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(""),
                })
                .Callback((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    _messages.Add(request);
                })
                .Verifiable();

            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(handlerMock.Object));

            var mockConnectionService = new Mock<IConnectionService>();
            mockConnectionService.Setup(_ => _.ListAsync(It.IsAny<IAgentContext>(), It.IsAny<ISearchQuery>(), It.IsAny<int>()))
                .Returns(Task.FromResult(new List<ConnectionRecord> {new ConnectionRecord()}));

            var httpMessageDispatcher = new HttpMessageDispatcher(clientFactory.Object);

            _messagingService =
                new DefaultMessageService(new Mock<ILogger<DefaultMessageService>>().Object, new[] { httpMessageDispatcher });
        }

        public async Task InitializeAsync()
        {
            try
            {
                await Wallet.CreateWalletAsync(Config, WalletCredentials);
            }
            catch (WalletExistsException)
            {
                // OK
            }

            _wallet = await Wallet.OpenWalletAsync(Config, WalletCredentials);
        }

        public async Task DisposeAsync()
        {
            if (_wallet != null) await _wallet.CloseAsync();
            await Wallet.DeleteWalletAsync(Config, WalletCredentials);
        }

        [Fact]
        public async Task PackAnon()
        {

            var message = new ConnectionInvitationMessage {RecipientKeys = new [] { "123" }}.ToByteArray();

            var my = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var anotherMy = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            
            var packed = await CryptoUtils.PackAsync(_wallet, anotherMy.VerKey, message, null);

            Assert.NotNull(packed);
        }

        [Fact]
        public async Task PackAuth()
        {

            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" }}.ToByteArray();

            var my = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var anotherMy = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            
            var packed = await CryptoUtils.PackAsync(_wallet, anotherMy.VerKey, message, my.VerKey);

            Assert.NotNull(packed);
        }

        [Fact]
        public async Task PackAndUnpackAnon()
        {

            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" }};

            var my = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var anotherMy = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            
            var packed = await CryptoUtils.PackAsync(_wallet, anotherMy.VerKey, message, null);
            var unpack = await CryptoUtils.UnpackAsync(_wallet, packed);

            Assert.NotNull(unpack);
            Assert.Null(unpack.SenderVerkey);
            Assert.NotNull(unpack.RecipientVerkey);
            Assert.Equal(unpack.RecipientVerkey, anotherMy.VerKey);
        }

        [Fact]
        public async Task PackAndUnpackAuth()
        {

            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" }}.ToByteArray();

            var my = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var anotherMy = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            
            var packed = await CryptoUtils.PackAsync(_wallet, anotherMy.VerKey, message, my.VerKey);
            var unpack = await CryptoUtils.UnpackAsync(_wallet, packed);

            var jObject = JObject.Parse(unpack.Message);

            Assert.NotNull(unpack);
            Assert.NotNull(unpack.SenderVerkey);
            Assert.NotNull(unpack.RecipientVerkey);
            Assert.Equal(unpack.RecipientVerkey, anotherMy.VerKey);
            Assert.Equal(unpack.SenderVerkey, my.VerKey);
            Assert.Equal(MessageTypes.ConnectionInvitation, jObject["@type"].ToObject<string>());
        }

        [Fact]
        public async Task UnpackToCustomType()
        {

            var message = new ConnectionInvitationMessage {RecipientKeys = new [] { "123" }};

            var my = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var anotherMy = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");

            var packed = await CryptoUtils.PackAsync(_wallet, anotherMy.VerKey, message, null);
            var unpack = await CryptoUtils.UnpackAsync<ConnectionInvitationMessage>(_wallet, packed);

            Assert.NotNull(unpack);
            Assert.Equal("123", unpack.RecipientKeys[0]);
        }

        [Fact]
        public async Task AuthPrepareMessageNoRoutingAsync()
        {
            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" } };

            var recipient = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var sender = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");

            var encrypted = await _messagingService.PrepareAsync(_wallet, message, recipient.VerKey, new string[0], sender.VerKey);

            var unpackRes = await CryptoUtils.UnpackAsync(_wallet, encrypted);
            var unpackMsg = JsonConvert.DeserializeObject<ConnectionInvitationMessage>(unpackRes.Message);

            Assert.NotNull(unpackMsg);
            Assert.True(unpackRes.SenderVerkey == sender.VerKey);
            Assert.True(unpackRes.RecipientVerkey == recipient.VerKey);
            Assert.Equal("123", unpackMsg.RecipientKeys[0]);
        }

        [Fact]
        public async Task AuthPrepareMessageRoutingAsync()
        {
            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" } };

            var recipient = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var sender = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var routingRecipient = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");

            var encrypted = await _messagingService.PrepareAsync(_wallet, message, recipient.VerKey, new[] { routingRecipient.VerKey }, sender.VerKey);

            var unpackRes = await CryptoUtils.UnpackAsync(_wallet, encrypted);
            var unpackMsg = JsonConvert.DeserializeObject<ForwardMessage>(unpackRes.Message);

            Assert.NotNull(unpackMsg);
            Assert.True(string.IsNullOrEmpty(unpackRes.SenderVerkey));
            Assert.True(unpackRes.RecipientVerkey == routingRecipient.VerKey);
            Assert.Equal(recipient.VerKey, unpackMsg.To);

            var unpackRes1 = await CryptoUtils.UnpackAsync(_wallet, unpackMsg.Message.GetUTF8Bytes());
            var unpackMsg1 = JsonConvert.DeserializeObject<ConnectionInvitationMessage>(unpackRes1.Message);

            Assert.NotNull(unpackMsg1);
            Assert.True(unpackRes1.SenderVerkey == sender.VerKey);
            Assert.True(unpackRes1.RecipientVerkey == recipient.VerKey);
            Assert.Equal("123", unpackMsg1.RecipientKeys[0]);
        }

        [Fact]
        public async Task AnonPrepareMessageNoRoutingAsync()
        {
            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" } };

            var recipient = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");

            var encrypted = await _messagingService.PrepareAsync(_wallet, message, recipient.VerKey);

            var unpackRes = await CryptoUtils.UnpackAsync(_wallet, encrypted);
            var unpackMsg = JsonConvert.DeserializeObject<ConnectionInvitationMessage>(unpackRes.Message);

            Assert.NotNull(unpackMsg);
            Assert.True(string.IsNullOrEmpty(unpackRes.SenderVerkey));
            Assert.True(unpackRes.RecipientVerkey == recipient.VerKey);
            Assert.Equal("123", unpackMsg.RecipientKeys[0]);
        }

        [Fact]
        public async Task AnonPrepareMessageRoutingAsync()
        {
            var message = new ConnectionInvitationMessage { RecipientKeys = new[] { "123" } };

            var recipient = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");
            var routingRecipient = await Did.CreateAndStoreMyDidAsync(_wallet, "{}");

            var encrypted = await _messagingService.PrepareAsync(_wallet, message, recipient.VerKey, new []{ routingRecipient.VerKey });

            var unpackRes = await CryptoUtils.UnpackAsync(_wallet, encrypted);
            var unpackMsg = JsonConvert.DeserializeObject<ForwardMessage>(unpackRes.Message);

            Assert.NotNull(unpackMsg);
            Assert.True(string.IsNullOrEmpty(unpackRes.SenderVerkey));
            Assert.True(unpackRes.RecipientVerkey == routingRecipient.VerKey);
            Assert.Equal(recipient.VerKey, unpackMsg.To);

            var unpackRes1 = await CryptoUtils.UnpackAsync(_wallet, unpackMsg.Message.GetUTF8Bytes());
            var unpackMsg1 = JsonConvert.DeserializeObject<ConnectionInvitationMessage>(unpackRes1.Message);

            Assert.NotNull(unpackMsg1);
            Assert.True(string.IsNullOrEmpty(unpackRes1.SenderVerkey));
            Assert.True(unpackRes1.RecipientVerkey == recipient.VerKey);
            Assert.Equal("123", unpackMsg1.RecipientKeys[0]);
        }

        [Fact]
        public async Task SendToConnectionAsyncThrowsInvalidMessageNoId()
        {
            var connection = new ConnectionRecord
            {
                Alias = new ConnectionAlias
                {
                    Name = "Test"
                },
                Endpoint = new AgentEndpoint
                {
                    Uri = "https://mock.com"
                },
                TheirVk = Guid.NewGuid().ToString()
            };

            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () =>
                await _messagingService.SendAsync(_wallet, new MockAgentMessage(), connection));
            Assert.True(ex.ErrorCode == ErrorCode.InvalidMessage);
        }

        [Fact]
        public async Task SendToConnectionAsyncThrowsInvalidMessageNoType()
        {
            var connection = new ConnectionRecord
            {
                Alias = new ConnectionAlias
                {
                    Name = "Test"
                },
                Endpoint = new AgentEndpoint
                {
                    Uri = "https://mock.com"
                },
                TheirVk = Guid.NewGuid().ToString()
            };

            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () =>
                await _messagingService.SendAsync(_wallet, new MockAgentMessage { Id = Guid.NewGuid().ToString() }, connection));
            Assert.True(ex.ErrorCode == ErrorCode.InvalidMessage);
        }
    }
}
