using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.EphemeralChallenge;
using AgentFramework.Core.Models.Credentials;
using AgentFramework.Core.Models.EphemeralChallenge;
using AgentFramework.Core.Models.Proofs;
using AgentFramework.Core.Models.Records;
using AgentFramework.TestHarness;
using AgentFramework.TestHarness.Utils;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AgentFramework.Core.Runtime;

namespace AgentFramework.Core.Tests.Protocols
{
    public class EphemeralChallengeTests : IAsyncLifetime
    {
        private readonly string _issuerConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _holderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _requestorConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";
        
        private IAgentContext _issuerWallet;
        private IAgentContext _holderWallet;
        private IAgentContext _requestorWallet;

        private readonly IConnectionService _connectionService;
        private readonly IProofService _proofService;
        private readonly ICredentialService _credentialService;
        private readonly IEphemeralChallengeService _ephemeralChallengeService;

        private readonly ISchemaService _schemaService;

        private bool _routeMessage = true;
        private readonly ConcurrentBag<AgentMessage> _messages = new ConcurrentBag<AgentMessage>();

        public EphemeralChallengeTests()
        {
            var recordService = new DefaultWalletRecordService();
            var ledgerService = new DefaultLedgerService();

            var eventAggregator = new EventAggregator();

            var routingMock = new Mock<IMessageService>();
            routingMock.Setup(x =>
                    x.SendAsync(It.IsAny<Wallet>(), It.IsAny<AgentMessage>(), It.IsAny<ConnectionRecord>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((Wallet _, AgentMessage content, ConnectionRecord __, string ___, bool ____) =>
                {
                    if (_routeMessage)
                        _messages.Add(content);
                    else
                        throw new AgentFrameworkException(ErrorCode.LedgerOperationRejected, "");
                })
                .Returns(Task.FromResult<MessageContext>(null));

            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            var provisioningMock = ServiceUtils.GetDefaultMockProvisioningService();
            var paymentService = new DefaultPaymentService();
            var tailsService = new DefaultTailsService(ledgerService, clientFactory.Object);

            _schemaService = new DefaultSchemaService(provisioningMock, recordService, ledgerService, paymentService, tailsService);

            _connectionService = new DefaultConnectionService(
                eventAggregator,
                recordService,
                provisioningMock,
                new Mock<ILogger<DefaultConnectionService>>().Object);

            _credentialService = new DefaultCredentialService(
                eventAggregator,
                ledgerService,
                _connectionService,
                recordService,
                _schemaService,
                tailsService,
                provisioningMock,
                paymentService,
                new Mock<ILogger<DefaultCredentialService>>().Object);

            _proofService = new DefaultProofService(
                eventAggregator,
                _connectionService,
                recordService,
                provisioningMock,
                ledgerService,
                tailsService,
                new Mock<ILogger<DefaultProofService>>().Object);

            _ephemeralChallengeService = new DefaultEphemeralChallengeService(eventAggregator, _proofService, recordService, provisioningMock, new Mock<ILogger<DefaultEphemeralChallengeService>>().Object);
        }

        public async Task InitializeAsync()
        {
            _issuerWallet = await AgentUtils.Create(_issuerConfig, Credentials, true);
            _holderWallet = await AgentUtils.Create(_holderConfig, Credentials, true);
            _requestorWallet = await AgentUtils.Create(_requestorConfig, Credentials, true);
        }

        [Fact]
        public async Task CanCreateChallengeConfigAsync()
        {
            var config = new EphemeralChallengeConfiguration
            {
                Name = "Test",
                Type = ChallengeType.Proof,
                Contents = new ProofRequestConfiguration
                {
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"", new ProofAttributeInfo {Name = "Test"}}
                    }
                }
            };

            var id = await _ephemeralChallengeService.CreateChallengeConfigAsync(_issuerWallet, config);

            var record = await _ephemeralChallengeService.GetChallengeConfigAsync(_issuerWallet, id);

            var result = record.Contents.ToObject<ProofRequestConfiguration>();

            Assert.True(result.RequestedAttributes.Count == 1);
            Assert.True(config.Type == record.Type);
            Assert.True(config.Name == record.Name);
        }

        [Fact]
        public async Task GetChallengeConfigAsyncThrowsRecordNotFound()
        {
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () =>
                await _ephemeralChallengeService.GetChallengeConfigAsync(_holderWallet, "bad-config-id"));

            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task GetChallengeAsyncThrowsRecordNotFound()
        {
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () =>
                await _ephemeralChallengeService.GetChallengeAsync(_holderWallet, "bad-config-id"));

            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task CanCreateChallengeAsync()
        {
            var config = new EphemeralChallengeConfiguration
            {
                Name = "Test",
                Type = ChallengeType.Proof,
                Contents = new ProofRequestConfiguration
                {
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"", new ProofAttributeInfo {Name = "Test"}}
                    }
                }
            };

            var id = await _ephemeralChallengeService.CreateChallengeConfigAsync(_holderWallet, config);

            (var challenge, var record) = await _ephemeralChallengeService.CreateChallengeAsync(_holderWallet, id);

            Assert.True(!string.IsNullOrEmpty(record.Id));
            Assert.True(challenge != null);
        }

        [Fact]
        public async Task CanConductChallengeFlow()
        {
            //Setup a connection and issue the credentials to the holder
            var (issuerConnection, holderConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            await Scenarios.IssueCredentialAsync(
                _schemaService, _credentialService, _messages, issuerConnection,
                holderConnection, _issuerWallet, _holderWallet, await _holderWallet.Pool, TestConstants.DefaultMasterSecret, true, new List<CredentialPreviewAttribute>
                {
                    new CredentialPreviewAttribute("first_name", "Test"),
                    new CredentialPreviewAttribute("last_name", "Holder")
                });

            _messages.Clear();

            // Challenger sends a challenge
            {
                var challengeConfig = new EphemeralChallengeConfiguration
                {
                    Name = "Test",
                    Type = ChallengeType.Proof,
                    Contents = new ProofRequestConfiguration
                    {
                        RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                        {
                            {"first-name-requirement", new ProofAttributeInfo {Name = "first_name"}}
                        }
                    }
                };

                var challengeConfigId = await _ephemeralChallengeService.CreateChallengeConfigAsync(_requestorWallet, challengeConfig);

                (var challenge, var record) = await _ephemeralChallengeService.CreateChallengeAsync(_requestorWallet, challengeConfigId);

                Assert.True(!string.IsNullOrEmpty(challenge.ChallengerName));
                Assert.True(challenge.RecipientKeys.Count() == 1);
                Assert.True(challenge.RecipientKeys.First() ==  TestConstants.DefaultVerkey);
                Assert.True(challenge.ServiceEndpoint == TestConstants.DefaultMockUri);

                _messages.Add(challenge);

                var result = await _ephemeralChallengeService.GetChallengeStateAsync(_requestorWallet, record.Id);
                Assert.True(result == ChallengeState.Challenged);
            }

            //Challenge responder recieves challenge
            {
                var challengeMessage = _messages.OfType<EphemeralChallengeMessage>().First();

                var proofRequest = challengeMessage.Challenge.Contents.ToObject<ProofRequest>();

                var requestedCredentials = new RequestedCredentials();
                foreach (var requestedAttribute in proofRequest.RequestedAttributes)
                {
                    var credentials =
                        await _proofService.ListCredentialsForProofRequestAsync(_holderWallet, proofRequest,
                            requestedAttribute.Key);

                    requestedCredentials.RequestedAttributes.Add(requestedAttribute.Key,
                        new RequestedAttribute
                        {
                            CredentialId = credentials.First().CredentialInfo.Referent,
                            Revealed = true,
                            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        });
                }

                foreach (var requestedAttribute in proofRequest.RequestedPredicates)
                {
                    var credentials =
                        await _proofService.ListCredentialsForProofRequestAsync(_holderWallet, proofRequest,
                            requestedAttribute.Key);

                    requestedCredentials.RequestedPredicates.Add(requestedAttribute.Key,
                        new RequestedAttribute
                        {
                            CredentialId = credentials.First().CredentialInfo.Referent,
                            Revealed = true
                        });
                }

                var challenge = await _ephemeralChallengeService.CreateProofChallengeResponseAsync(
                    _holderWallet, challengeMessage, requestedCredentials);

                _messages.Add(challenge);
            }

            //Challenger recieves challenge response and verifies it
            {
                var challengeResponseMessage = _messages.OfType<EphemeralChallengeResponseMessage>().First();

                var id = await _ephemeralChallengeService.ProcessChallengeResponseAsync(_requestorWallet, challengeResponseMessage);

                var result = await _ephemeralChallengeService.GetChallengeStateAsync(_requestorWallet, id);
                Assert.True(result == ChallengeState.Accepted);
            }
        }

        public async Task DisposeAsync()
        {
            if (_issuerWallet != null) await _issuerWallet.Wallet.CloseAsync();
            if (_holderWallet != null) await _holderWallet.Wallet.CloseAsync();
            if (_requestorWallet != null) await _requestorWallet.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(_issuerConfig, Credentials);
            await Wallet.DeleteWalletAsync(_holderConfig, Credentials);
            await Wallet.DeleteWalletAsync(_requestorConfig, Credentials);
        }
    }
}