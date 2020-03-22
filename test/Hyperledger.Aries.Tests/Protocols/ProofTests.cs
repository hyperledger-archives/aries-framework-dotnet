﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Models.Events;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Utils;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Hyperledger.Aries.Runtime;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Payments;
using Hyperledger.Aries.Storage;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class ProofTests : IAsyncLifetime
    {
        static ProofTests()
        {
            global::Hyperledger.Aries.Utils.Runtime.SetFlags(Hyperledger.Aries.Utils.Runtime.LedgerLookupRetryFlag);
        }

        private readonly string IssuerConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string HolderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string RequestorConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string WalletCredentials = "{\"key\":\"test_wallet_key\"}";

        private IAgentContext _issuerWallet;
        private IAgentContext _holderWallet;
        private IAgentContext _requestorWallet;

        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectionService _connectionService;
        private readonly ICredentialService _credentialService;
        private readonly IProofService _proofService;

        private readonly ISchemaService _schemaService;

        private readonly ConcurrentBag<AgentMessage> _messages = new ConcurrentBag<AgentMessage>();

        public ProofTests()
        {
            var recordService = new DefaultWalletRecordService();
            var ledgerService = new DefaultLedgerService(new DefaultLedgerSigningService());

            _eventAggregator = new EventAggregator();

            var messageService = new DefaultMessageService(new Mock<ILogger<DefaultMessageService>>().Object, new IMessageDispatcher[] { });

            var provisioning = ServiceUtils.GetDefaultMockProvisioningService();
            var paymentService = new DefaultPaymentService();

            var clientFactory = new Mock<IHttpClientFactory>();
            clientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            var tailsService = new DefaultTailsService(ledgerService, clientFactory.Object);
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
                new Mock<ILogger<DefaultCredentialService>>().Object);

            _proofService = new DefaultProofService(
                _eventAggregator,
                _connectionService,
                recordService,
                provisioning,
                ledgerService,
                tailsService,
                messageService,
                new Mock<ILogger<DefaultProofService>>().Object);
        }

        public async Task InitializeAsync()
        {
            _issuerWallet = await AgentUtils.Create(IssuerConfig, WalletCredentials, true);
            _holderWallet = await AgentUtils.Create(HolderConfig, WalletCredentials, true);
            _requestorWallet = await AgentUtils.Create(RequestorConfig, WalletCredentials, true);
        }

        [Fact]
        public async Task CredentialProofDemo()
        {
            var events = 0;
            _eventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
                .Where(_ => (_.MessageType == MessageTypes.PresentProofNames.RequestPresentation ||
                             _.MessageType == MessageTypes.PresentProofNames.Presentation))
                .Subscribe(_ =>
                {
                    events++;
                });

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

            //Requestor initialize a connection with the holder
            var (holderRequestorConnection, requestorConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _holderWallet, _requestorWallet);

            await Scenarios.ProofProtocolAsync(_proofService, _messages, holderRequestorConnection, requestorConnection,
                _holderWallet, _requestorWallet, new ProofRequest()
                {
                    Name = "ProofReq",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"first-name-requirement", new ProofAttributeInfo {Name = "first_name"}}
                    }
                });

            _messages.Clear();

            Assert.True(events == 2);
        }

        [Fact]
        public async Task SendProofRequestThrowsConnectionNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.CreateRequestAsync(_issuerWallet, new ProofRequest
            {
                Name = "Test",
                Nonce = await AnonCreds.GenerateNonceAsync()
            }, "bad-proof-id"));

            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task SendProofRequestThrowsConnectionInvalidState()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId });

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.CreateRequestAsync(_issuerWallet, new ProofRequest
            {
                Name = "Test",
                Nonce = await AnonCreds.GenerateNonceAsync()
            }, connectionId));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task CreateProofRequestConnectionNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.CreateRequestAsync(_issuerWallet, new ProofRequest
            {
                Name = "Test",
                Nonce = await AnonCreds.GenerateNonceAsync()
            }, "bad-connection-id"));

            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task CreateProofRequestConnectionInvalidState()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId });

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.CreateRequestAsync(_issuerWallet, new ProofRequest
            {
                Name = "Test",
                Nonce = await AnonCreds.GenerateNonceAsync()
            }, connectionId));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task ProcessProofRecordNotFound()
        {
            var (issuerConnection, _) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () =>
                await _proofService.ProcessPresentationAsync(_issuerWallet, new PresentationMessage()));

            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task ProcessProofInvalidState()
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

            //Requestor initialize a connection with the holder
            var (_, requestorConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _holderWallet, _requestorWallet);

            // Verifier sends a proof request to prover
            {
                var proofRequestObject = new ProofRequest
                {
                    Name = "ProofReq",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"first-name-requirement", new ProofAttributeInfo {Name = "first_name"}}
                    }
                };

                //Requestor sends a proof request
                var (message, _) = await _proofService.CreateRequestAsync(_requestorWallet, proofRequestObject, requestorConnection.Id);
                _messages.Add(message);
            }

            // Holder accepts the proof requests and builds a proof
            {
                //Holder retrives proof request message from their cloud agent
                var proofRequest = FindContentMessage<RequestPresentationMessage>();
                Assert.NotNull(proofRequest);

                //Holder stores the proof request
                var holderProofRequestId = await _proofService.ProcessRequestAsync(_holderWallet, proofRequest, holderConnection);
                var holderProofRecord = await _proofService.GetAsync(_holderWallet, holderProofRequestId.Id);
                var holderProofObject =
                    JsonConvert.DeserializeObject<ProofRequest>(holderProofRecord.RequestJson);

                var requestedCredentials = new RequestedCredentials();
                foreach (var requestedAttribute in holderProofObject.RequestedAttributes)
                {
                    var credentials =
                        await _proofService.ListCredentialsForProofRequestAsync(_holderWallet, holderProofObject,
                            requestedAttribute.Key);

                    requestedCredentials.RequestedAttributes.Add(requestedAttribute.Key,
                        new RequestedAttribute
                        {
                            CredentialId = credentials.First().CredentialInfo.Referent,
                            Revealed = true
                        });
                }

                foreach (var requestedAttribute in holderProofObject.RequestedPredicates)
                {
                    var credentials =
                        await _proofService.ListCredentialsForProofRequestAsync(_holderWallet, holderProofObject,
                            requestedAttribute.Key);

                    requestedCredentials.RequestedPredicates.Add(requestedAttribute.Key,
                        new RequestedAttribute
                        {
                            CredentialId = credentials.First().CredentialInfo.Referent,
                            Revealed = true
                        });
                }

                //Holder accepts the proof request and sends a proof
                (var proofMessage, var _) = await _proofService.CreatePresentationAsync(_holderWallet, holderProofRequestId.Id,
                    requestedCredentials);
                _messages.Add(proofMessage);
            }

            //Requestor retrives proof message from their cloud agent
            var proof = FindContentMessage<PresentationMessage>();
            Assert.NotNull(proof);

            //Requestor stores proof
            await _proofService.ProcessPresentationAsync(_requestorWallet, proof);

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.ProcessPresentationAsync(_requestorWallet, proof));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task AcceptProofRequestCredentialNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.CreatePresentationAsync(_issuerWallet, "bad-proof-id", null));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task AcceptProofRequestCredentialInvalidState()
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

            //Requestor initialize a connection with the holder
            var (_, requestorConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _holderWallet, _requestorWallet);

            // Verifier sends a proof request to prover
            {
                var proofRequestObject = new ProofRequest
                {
                    Name = "ProofReq",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"first-name-requirement", new ProofAttributeInfo {Name = "first_name"}}
                    }
                };

                //Requestor sends a proof request
                var (message, _) = await _proofService.CreateRequestAsync(_requestorWallet, proofRequestObject, requestorConnection.Id);
                _messages.Add(message);
            }

            // Holder accepts the proof requests and builds a proof
            //Holder retrives proof request message from their cloud agent
            var proofRequest = FindContentMessage<RequestPresentationMessage>();
            Assert.NotNull(proofRequest);

            //Holder stores the proof request
            var holderProofRequestId = await _proofService.ProcessRequestAsync(_holderWallet, proofRequest, holderConnection);
            var holderProofRecord = await _proofService.GetAsync(_holderWallet, holderProofRequestId.Id);
            var holderProofObject =
                JsonConvert.DeserializeObject<ProofRequest>(holderProofRecord.RequestJson);

            var requestedCredentials = new RequestedCredentials();
            foreach (var requestedAttribute in holderProofObject.RequestedAttributes)
            {
                var credentials =
                    await _proofService.ListCredentialsForProofRequestAsync(_holderWallet, holderProofObject,
                        requestedAttribute.Key);

                requestedCredentials.RequestedAttributes.Add(requestedAttribute.Key,
                    new RequestedAttribute
                    {
                        CredentialId = credentials.First().CredentialInfo.Referent,
                        Revealed = true
                    });
            }

            foreach (var requestedAttribute in holderProofObject.RequestedPredicates)
            {
                var credentials =
                    await _proofService.ListCredentialsForProofRequestAsync(_holderWallet, holderProofObject,
                        requestedAttribute.Key);

                requestedCredentials.RequestedPredicates.Add(requestedAttribute.Key,
                    new RequestedAttribute
                    {
                        CredentialId = credentials.First().CredentialInfo.Referent,
                        Revealed = true
                    });
            }

            //Holder accepts the proof request and sends a proof
            await _proofService.CreatePresentationAsync(_holderWallet, holderProofRequestId.Id, requestedCredentials);
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.CreatePresentationAsync(_holderWallet, holderProofRequestId.Id,
                requestedCredentials));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task RejectProofRequestCredentialNotFound()
        {
            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.RejectProofRequestAsync(_issuerWallet, "bad-proof-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task RejectProofRequestCredentialInvalidState()
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

            //Requestor initialize a connection with the holder
            var (_, requestorConnection) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _holderWallet, _requestorWallet);

            // Verifier sends a proof request to prover
            {
                var proofRequestObject = new ProofRequest
                {
                    Name = "ProofReq",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        {"first-name-requirement", new ProofAttributeInfo {Name = "first_name"}}
                    }
                };

                //Requestor sends a proof request
                var (message, _) = await _proofService.CreateRequestAsync(_requestorWallet, proofRequestObject, requestorConnection.Id);
                _messages.Add(message);
            }

            //Holder retrieves proof request message from their cloud agent
            var proofRequest = FindContentMessage<RequestPresentationMessage>();
            Assert.NotNull(proofRequest);

            //Holder stores the proof request
            var holderProofRequestId = await _proofService.ProcessRequestAsync(_holderWallet, proofRequest, holderConnection);

            //Holder accepts the proof request and sends a proof
            await _proofService.RejectProofRequestAsync(_holderWallet, holderProofRequestId.Id);

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _proofService.RejectProofRequestAsync(_holderWallet, holderProofRequestId.Id));
            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        private T FindContentMessage<T>() where T : AgentMessage
            => _messages.OfType<T>().Single();

        public async Task DisposeAsync()
        {
            if (_issuerWallet != null) await _issuerWallet.Wallet.CloseAsync();
            if (_holderWallet != null) await _holderWallet.Wallet.CloseAsync();
            if (_requestorWallet != null) await _requestorWallet.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(IssuerConfig, WalletCredentials);
            await Wallet.DeleteWalletAsync(HolderConfig, WalletCredentials);
            await Wallet.DeleteWalletAsync(RequestorConfig, WalletCredentials);
        }
    }
}