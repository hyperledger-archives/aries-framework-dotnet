using System;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.Handshakes;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Hyperledger.TestHarness.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class OutOfBandTests : IAsyncLifetime
    {
        private readonly string _senderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _receiverConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";
        
        private IAgentContext _sender;
        private IAgentContext _receiver;

        private readonly IOutOfBandService _outOfBandService;
        private readonly Mock<IEventAggregator> _eventAggregator;

        public OutOfBandTests()
        {
            _eventAggregator = new Mock<IEventAggregator>();
            var provisioningService = ServiceUtils.GetDefaultMockProvisioningService();
            var connectionService = new DefaultConnectionService(_eventAggregator.Object,
                new DefaultWalletRecordService(), provisioningService,
                new Mock<ILogger<DefaultConnectionService>>().Object);
            var didExchangeService = new DefaultDidExchangeService(new Mock<ILedgerService>().Object,
                new DefaultWalletRecordService(), provisioningService, _eventAggregator.Object);
                
            _outOfBandService = new DefaultOutOfBandService(_eventAggregator.Object, connectionService, provisioningService, didExchangeService, new DefaultWalletRecordService());
        }
        
        public async Task InitializeAsync()
        {
            _sender = await AgentUtils.Create(_senderConfig, Credentials);
            _receiver = await AgentUtils.Create(_receiverConfig, Credentials);
        }

        [Fact]
        public void CanDeserializeInvitationWithoutRequests()
        {
            const string invitationJson = 
            @"{
                ""@type"": ""https://didcomm.org/out-of-band/1.1/invitation"",
                ""@id"": ""98765"",
                ""label"": ""Faber College"",
                ""handshake_protocols"": [""https://didcomm.org/didexchange/1.0""],
                ""services"": [
                    {
                        ""id"": ""#inline"",
                        ""type"": ""did-communication"",
                        ""recipientKeys"": [""did:key:z6MkpTHR8VNsBxYAAWHut2Geadd9jSwuBV8xRoAnwWsdvktH""],
                        ""routingKeys"": [],
                        ""serviceEndpoint"": ""https://example.com:5000""
                    },
                    ""did:sov:LjgpST2rjsoxYegQDRm7EL""
                ]
            }";
            
            var invitation = invitationJson.ToObject<InvitationMessage>();
            
            Assert.NotNull(invitation);
            Assert.Null(invitation.AttachedRequests);
            Assert.Equal("98765", invitation.Id);
            Assert.Equal("Faber College", invitation.Label);
            Assert.Equal(HandshakeProtocolUri.DidExchange, invitation.HandshakeProtocols.First());
            Assert.Null(invitation.GoalCode);
            Assert.Null(invitation.Goal);
            Assert.Null(invitation.Accept);
            Assert.Null(invitation.AttachedRequests);
            Assert.Equal("did:sov:LjgpST2rjsoxYegQDRm7EL", invitation.Services[1]);
        }

        [Fact]
        public void CanDeserializeInvitationWithRequest()
        {
            const string invitationJson = 
            @"{
                ""@type"": ""https://didcomm.org/out-of-band/1.1/invitation"",
                ""@id"": ""12345"",
                ""label"": ""Faber College"",
                ""goal_code"": ""issue-vc"",
                ""goal"": ""To issue a Faber College Graduate credential"",
                ""accept"": [
                    ""didcomm/aip2;env=rfc587"",
                    ""didcomm/aip2;env=rfc19""
                ],
                ""handshake_protocols"": [
                    ""https://didcomm.org/didexchange/1.0"",
                    ""https://didcomm.org/connections/1.0""
                ],
                ""requests~attach"": [
                    {
                        ""@id"": ""request-0"",
                        ""mime-type"": ""application/json"",
                        ""data"": {
                            ""json"": ""{}""
                        }
                    }
                ],
                ""services"": [""did:sov:LjgpST2rjsoxYegQDRm7EL""]
            }";
            
            var invitation = invitationJson.ToObject<InvitationMessage>();
            
            Assert.NotNull(invitation);
            Assert.NotNull(invitation.AttachedRequests);
            Assert.Equal("12345", invitation.Id);
            Assert.Equal("Faber College", invitation.Label);
            Assert.Equal("issue-vc", invitation.GoalCode);
            Assert.Equal("To issue a Faber College Graduate credential", invitation.Goal);
            Assert.Equal("didcomm/aip2;env=rfc19", invitation.Accept.Last());
            Assert.Equal("request-0", invitation.AttachedRequests.First().Id);
            Assert.Equal("did:sov:LjgpST2rjsoxYegQDRm7EL", invitation.Services.First());
        }

        [Fact]
        public async Task CanCreateInvitation()
        {
            var (invitation, record) = await _outOfBandService.CreateInvitationAsync(_sender, null);
            
            Assert.NotNull(invitation);
            Assert.Null(invitation.AttachedRequests);
        }
        
        [Fact]
        public async Task CanCreateInvitationWithRequest()
        {
            var result = await _outOfBandService.CreateInvitationAsync(_sender, null);
            
            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
        }

        [Fact]
        public async Task CanProcessInvitation()
        {
            var (invitation, record) = await _outOfBandService.CreateInvitationAsync(_sender);

            await _outOfBandService.ProcessInvitationMessage(_receiver, invitation);

            _eventAggregator.Verify( x => x.Publish(It.IsAny<ServiceMessageProcessingEvent>()), Times.Once);
        }

        public async Task DisposeAsync()
        {
            if (_sender != null) await _sender.Wallet.CloseAsync();
            if (_receiver != null) await _receiver.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(_senderConfig, Credentials);
            await Wallet.DeleteWalletAsync(_receiverConfig, Credentials);
        }
    }
}
