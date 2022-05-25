using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Runtime;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.TestHarness;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class CredentialTests : IAsyncLifetime
    {
        static CredentialTests()
        {
            global::Hyperledger.Aries.Utils.Runtime.SetFlags(Hyperledger.Aries.Utils.Runtime.LedgerLookupRetryFlag);
        }

        private readonly string _issuerConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _holderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private IAgentContext _issuerWallet;
        private IAgentContext _holderWallet;

        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly ISchemaService _schemaService;

        private readonly ConcurrentBag<AgentMessage> _messages = new ConcurrentBag<AgentMessage>();

        public CredentialTests()
        {
            var recordService = new DefaultWalletRecordService();
            var ledgerService = new DefaultLedgerService(new DefaultLedgerSigningService(new DefaultProvisioningService(recordService, new DefaultWalletService(), Options.Create(new AgentOptions()))));

            var messageService = new DefaultMessageService(new Mock<ILogger<DefaultMessageService>>().Object, new IMessageDispatcher[] { });

            _eventAggregator = new EventAggregator();

            var provisioning = ServiceUtils.GetDefaultMockProvisioningService();
            var paymentService = new DefaultPaymentService();

            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            var tailsService = new DefaultTailsService(ledgerService, Options.Create(new Configuration.AgentOptions()), clientFactory.Object);
            _schemaService = new DefaultSchemaService(provisioning, recordService, ledgerService, paymentService, tailsService, Options.Create(new Configuration.AgentOptions()));

            _connectionService = new DefaultConnectionService(
                _eventAggregator,
                recordService,
                provisioning,
                new Mock<ILogger<DefaultConnectionService>>().Object);

            _credentialService = new DefaultCredentialService(
                _eventAggregator,
                ledgerService,
                _connectionService,
                recordService,
                _schemaService,
                tailsService,
                provisioning,
                paymentService,
                messageService,
                new Mock<ILogger<DefaultCredentialService>>().Object,
                null);
        }

        public async Task InitializeAsync()
        {
            _issuerWallet = await AgentUtils.Create(_issuerConfig, Credentials, true);
            _holderWallet = await AgentUtils.Create(_holderConfig, Credentials, true);
        }

        /// <summary>
        /// This test requires a local running node accessible at 127.0.0.1
        /// </summary>
        /// <returns>The issuance demo.</returns>
        [Fact]
        public async Task CanIssueCredential()
        {
            var events = 0;
            _eventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
                .Where(_ => (_.MessageType == MessageTypes.IssueCredentialNames.OfferCredential ||
                             _.MessageType == MessageTypes.IssueCredentialNames.RequestCredential ||
                             _.MessageType == MessageTypes.IssueCredentialNames.IssueCredential))
                .Subscribe(_ =>
                {
                    events++;
                });

            // Setup secure connection between issuer and holder
            var (issuerConnection, holderConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            var (issuerCredential, holderCredential) = await Scenarios.IssueCredentialAsync(
                _schemaService, _credentialService, _messages, issuerConnection,
                holderConnection, _issuerWallet, _holderWallet, await _holderWallet.Pool, TestConstants.DefaultMasterSecret, false, new List<CredentialPreviewAttribute>
                {
                    new CredentialPreviewAttribute("first_name", "Test"),
                    new CredentialPreviewAttribute("last_name", "Holder")
                });

            Assert.True(events == 3);

            Assert.Equal(issuerCredential.State, holderCredential.State);
            Assert.Equal(CredentialState.Issued, issuerCredential.State);
        }

        [Fact]
        public async Task CanStoreAndReceiveImagePngMimeTypes()
        {
            const string pngFile = "base64_encoded_png_image_file";
            
            var (issuerConnection, holderConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            var (issuerCredential, holderCredential) = await Scenarios.IssueCredentialAsync(
                _schemaService, _credentialService, _messages, issuerConnection,
                holderConnection, _issuerWallet, _holderWallet, await _holderWallet.Pool, TestConstants.DefaultMasterSecret, false, new List<CredentialPreviewAttribute>
                {
                    new CredentialPreviewAttribute
                    {
                        MimeType = CredentialMimeTypes.ImagePngMimeType,
                        Name = "preview_image",
                        Value = pngFile
                    }
                });

            var actualResult = string.Empty;
            foreach (var credentialPreviewAttribute in holderCredential.CredentialAttributesValues)
            {
                if (credentialPreviewAttribute.MimeType == CredentialMimeTypes.ImagePngMimeType)
                    actualResult = credentialPreviewAttribute.Value as string;
            }
            
            Assert.Equal(pngFile, actualResult);
        }
        

        [Fact]
        public async Task CanCreateCredentialOffer()
        {
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            var result = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(_issuerWallet, _schemaService, issuer.Did,
                new[] { "test-attr" });

            var (msg, credentialRecord) = await _credentialService.CreateOfferAsync(_issuerWallet,
                new OfferConfiguration { CredentialDefinitionId = result.Item1 }, null);

            Assert.Equal(CredentialState.Offered, credentialRecord.State);
            Assert.NotNull(msg);
            Assert.Null(msg.CredentialPreview);
        }

        [Fact]
        public async Task CanCreateCredentialOfferWithPreview()
        {
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            var result = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(_issuerWallet, _schemaService, issuer.Did,
                new[] { "test-attr" });

            var (msg, credentialRecord) = await _credentialService.CreateOfferAsync(_issuerWallet,
                new OfferConfiguration
                {
                    CredentialDefinitionId = result.Item1,
                    CredentialAttributeValues = new List<CredentialPreviewAttribute>
                    {
                        new CredentialPreviewAttribute("test-attr","test-attr-value")
                    }
                });

            Assert.Equal(CredentialState.Offered, credentialRecord.State);
            Assert.NotNull(msg);
            Assert.NotNull(msg.CredentialPreview);
            Assert.True(msg.CredentialPreview.Attributes.Count() == 1);

            var previewAttr = msg.CredentialPreview.Attributes.ToArray()[0];

            Assert.True(previewAttr.Name == "test-attr");
            Assert.True(previewAttr.MimeType == CredentialMimeTypes.TextMimeType);
            Assert.True((string)previewAttr.Value == "test-attr-value");
        }

        [Fact]
        public async Task CreateCredentialOfferWithBadAttributeValuesThrowsException()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateOfferAsync(_issuerWallet,
                new OfferConfiguration
                {
                    CredentialAttributeValues = new List<CredentialPreviewAttribute>
                    {
                        new CredentialPreviewAttribute("test-attr","test-attr-value")
                        {
                            MimeType = "bad-mime-type"
                        }
                    }
                }));

            Assert.True(ex.ErrorCode == ErrorCode.InvalidParameterFormat);
        }

        [Fact]
        public async Task CreateCredentialOfferWithMultipleBadAttributeValuesThrowsException()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateOfferAsync(_issuerWallet,
                new OfferConfiguration
                {
                    CredentialAttributeValues = new List<CredentialPreviewAttribute>
                    {
                        new CredentialPreviewAttribute("test-attr","test-attr-value")
                        {
                            MimeType = "bad-mime-type"
                        },
                        new CredentialPreviewAttribute("test-attr1","test-attr-value1")
                        {
                            MimeType = "bad-mime-type"
                        }
                    }
                }));

            Assert.True(ex.ErrorCode == ErrorCode.InvalidParameterFormat);
            Assert.True(ex.Message.Split('\n').Count() == 2);
        }

        [Fact]
        public async Task RevokeCredentialOfferThrowsCredentialNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.RejectOfferAsync(_issuerWallet, "bad-connection-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task RevokeCredentialOfferThrowsCredentialInvalidState()
        {
            //Establish a connection between the two parties
            var (issuerConnection, holderConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            // Create an issuer DID/VK. Can also be created during provisioning
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            // Create a schema and credential definition for this issuer
            var (definitionId, _) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(_issuerWallet, _schemaService, issuer.Did,
                new[] { "dummy_attr" });

            var offerConfig = new OfferConfiguration
            {
                IssuerDid = issuer.Did,
                CredentialDefinitionId = definitionId
            };

            // Send an offer to the holder using the established connection channel
            var (offer, _) = await _credentialService.CreateOfferAsync(_issuerWallet, offerConfig, issuerConnection.Id);
            _messages.Add(offer);

            // Holder retrieves message from their cloud agent
            var credentialOffer = FindContentMessage<CredentialOfferMessage>(_messages);

            // Holder processes the credential offer by storing it
            var holderCredentialId =
                await _credentialService.ProcessOfferAsync(_holderWallet, credentialOffer, holderConnection);

            // Holder creates master secret. Will also be created during wallet agent provisioning
            await AnonCreds.ProverCreateMasterSecretAsync(_holderWallet.Wallet, TestConstants.DefaultMasterSecret);

            // Holder accepts the credential offer and sends a credential request
            var (request, _) = await _credentialService.CreateRequestAsync(_holderWallet, holderCredentialId);
            _messages.Add(request);

            // Issuer retrieves credential request from cloud agent
            var credentialRequest = FindContentMessage<CredentialRequestMessage>(_messages);
            Assert.NotNull(credentialRequest);

            // Issuer processes the credential request by storing it
            var issuerCredentialId =
                await _credentialService.ProcessCredentialRequestAsync(_issuerWallet, credentialRequest, issuerConnection);

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.RejectOfferAsync(_issuerWallet, issuerCredentialId));
            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task CreateOfferV1AsyncThrowsExceptionConnectionNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateOfferAsync(_issuerWallet, new OfferConfiguration(), "bad-connection-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task CreateOfferV1AsyncThrowsExceptionConnectionInvalidState()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId, AutoAcceptConnection = false });

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateOfferAsync(_issuerWallet, new OfferConfiguration(), connectionId));
            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task SendOfferAsyncThrowsExceptionConnectionNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateOfferAsync(_issuerWallet, new OfferConfiguration(), "bad-connection-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task SendOfferAsyncThrowsExceptionConnectionInvalidState()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId, AutoAcceptConnection = false });

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () =>
                await _credentialService.CreateOfferAsync(_issuerWallet, new OfferConfiguration(), connectionId));
            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task ProcessCredentialRequestThrowsCredentialNotFound()
        {
            var (issuerConnection, _) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.ProcessCredentialRequestAsync(_issuerWallet,
                new CredentialRequestMessage(), issuerConnection));

            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task RejectCredentialRequestThrowsExceptionCredentialNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.RejectCredentialRequestAsync(_holderWallet, "bad-credential-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task RejectCredentialRequestThrowsExceptionCredentialInvalidState()
        {
            //Establish a connection between the two parties
            var (issuerConnection, holderConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            // Create an issuer DID/VK. Can also be created during provisioning
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            // Create a schema and credential definition for this issuer
            var (definitionId, _) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(_issuerWallet, _schemaService, issuer.Did,
                new[] { "dummy_attr" });

            var offerConfig = new OfferConfiguration
            {
                IssuerDid = issuer.Did,
                CredentialDefinitionId = definitionId
            };

            // Send an offer to the holder using the established connection channel
            var (offerMessage, _) = await _credentialService.CreateOfferAsync(_issuerWallet, offerConfig, issuerConnection.Id);
            _messages.Add(offerMessage);

            // Holder retrieves message from their cloud agent
            var credentialOffer = FindContentMessage<CredentialOfferMessage>(_messages);

            // Holder processes the credential offer by storing it
            var holderCredentialId =
                await _credentialService.ProcessOfferAsync(_holderWallet, credentialOffer, holderConnection);

            // Holder creates master secret. Will also be created during wallet agent provisioning
            await AnonCreds.ProverCreateMasterSecretAsync(_holderWallet.Wallet, TestConstants.DefaultMasterSecret);

            // Holder accepts the credential offer and sends a credential request
            (var request, var _) = await _credentialService.CreateRequestAsync(_holderWallet, holderCredentialId);
            _messages.Add(request);

            // Issuer retrieves credential request from cloud agent
            var credentialRequest = FindContentMessage<CredentialRequestMessage>(_messages);
            Assert.NotNull(credentialRequest);

            // Issuer processes the credential request by storing it
            var issuerCredentialId =
                await _credentialService.ProcessCredentialRequestAsync(_issuerWallet, credentialRequest, issuerConnection);

            await _credentialService.RejectCredentialRequestAsync(_issuerWallet, issuerCredentialId);

            //Try reject the credential request again
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.RejectCredentialRequestAsync(_issuerWallet, issuerCredentialId));
            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task IssueCredentialThrowsExceptionCredentialNotFound()
        {
            // Create an issuer DID/VK. Can also be created during provisioning
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateCredentialAsync(_issuerWallet, "bad-credential-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task IssueCredentialThrowsExceptionCredentialInvalidState()
        {
            //Establish a connection between the two parties
            var (issuerConnection, holderConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            // Create an issuer DID/VK. Can also be created during provisioning
            var issuer = await Did.CreateAndStoreMyDidAsync(_issuerWallet.Wallet,
                new { seed = TestConstants.StewardSeed }.ToJson());

            // Create a schema and credential definition for this issuer
            var (definitionId, _) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(_issuerWallet, _schemaService, issuer.Did,
                new[] { "dummy_attr" });

            var offerConfig = new OfferConfiguration
            {
                IssuerDid = issuer.Did,
                CredentialDefinitionId = definitionId
            };

            // Send an offer to the holder using the established connection channel
            var (message, _) = await _credentialService.CreateOfferAsync(_issuerWallet, offerConfig, issuerConnection.Id);
            _messages.Add(message);

            // Holder retrieves message from their cloud agent
            var credentialOffer = FindContentMessage<CredentialOfferMessage>(_messages);

            // Holder processes the credential offer by storing it
            var holderCredentialId =
                await _credentialService.ProcessOfferAsync(_holderWallet, credentialOffer, holderConnection);

            // Holder creates master secret. Will also be created during wallet agent provisioning
            await AnonCreds.ProverCreateMasterSecretAsync(_holderWallet.Wallet, TestConstants.DefaultMasterSecret);

            // Holder accepts the credential offer and sends a credential request
            var (request, _) = await _credentialService.CreateRequestAsync(_holderWallet, holderCredentialId);
            _messages.Add(request);

            // Issuer retrieves credential request from cloud agent
            var credentialRequest = FindContentMessage<CredentialRequestMessage>(_messages);
            Assert.NotNull(credentialRequest);

            // Issuer processes the credential request by storing it
            var issuerCredentialId =
                await _credentialService.ProcessCredentialRequestAsync(_issuerWallet, credentialRequest, issuerConnection);

            // Issuer accepts the credential requests and issues a credential
            var (credential, _) = await _credentialService.CreateCredentialAsync(_issuerWallet, issuerCredentialId,
                new List<CredentialPreviewAttribute> { new CredentialPreviewAttribute("dummy_attr", "dummyVal") });
            _messages.Add(credential);

            //Try issue the credential again
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.CreateCredentialAsync(_issuerWallet, issuerCredentialId));
            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task RejectOfferAsyncThrowsExceptionCredentialOfferNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _credentialService.RejectOfferAsync(_issuerWallet, "bad-credential-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        private static T FindContentMessage<T>(IEnumerable<AgentMessage> collection)
            where T : AgentMessage
            => collection.OfType<T>().Single();

        public async Task DisposeAsync()
        {
            if (_issuerWallet != null) await _issuerWallet.Wallet.CloseAsync();
            if (_holderWallet != null) await _holderWallet.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(_issuerConfig, Credentials);
            await Wallet.DeleteWalletAsync(_holderConfig, Credentials);
        }
    }
}
