using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;
using AgentFramework.Core.Messages.Connections;
using AgentFramework.Core.Models.Connections;
using AgentFramework.Core.Models.Events;
using AgentFramework.Core.Models.Records;
using AgentFramework.Core.Runtime;
using AgentFramework.TestHarness;
using AgentFramework.TestHarness.Utils;
using Hyperledger.Indy.WalletApi;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AgentFramework.Core.Tests.Protocols
{
    public class ConnectionTests : IAsyncLifetime
    {
        private readonly string _issuerConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}"; 
        private readonly string _holderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _holderConfigTwo = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private IAgentContext _issuerWallet;
        private IAgentContext _holderWallet;
        private IAgentContext _holderWalletTwo;

        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectionService _connectionService;
        private readonly IProvisioningService _provisioningService;

        private readonly ConcurrentBag<AgentMessage> _messages = new ConcurrentBag<AgentMessage>();

        public ConnectionTests()
        {
            _eventAggregator = new EventAggregator();
            _provisioningService = ServiceUtils.GetDefaultMockProvisioningService();
            _connectionService = new DefaultConnectionService(
                _eventAggregator,
                new DefaultWalletRecordService(),
                _provisioningService,
                new Mock<ILogger<DefaultConnectionService>>().Object);
        }

        public async Task InitializeAsync()
        {
            _issuerWallet = await AgentUtils.Create(_issuerConfig, Credentials);
            _holderWallet = await AgentUtils.Create(_holderConfig, Credentials);
            _holderWalletTwo = await AgentUtils.Create(_holderConfigTwo, Credentials);
        }

        [Fact]
        public async Task CanCreateInvitationAsync()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration {ConnectionId = connectionId});

            var connection = await _connectionService.GetAsync(_issuerWallet, connectionId);

            Assert.False(connection.MultiPartyInvitation);
            Assert.Equal(ConnectionState.Invited, connection.State);
            Assert.Equal(connectionId, connection.Id);
        }

        [Fact]
        public async Task CanCreateMultiPartyInvitationAsync()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId, MultiPartyInvitation = true });

            var connection = await _connectionService.GetAsync(_issuerWallet, connectionId);

            Assert.True(connection.MultiPartyInvitation);
            Assert.Equal(ConnectionState.Invited, connection.State);
            Assert.Equal(connectionId, connection.Id);
        }

        [Fact]
        public async Task CreateInvitiationThrowsInvalidStateNoEndpoint()
        {
            var provisioningService = ServiceUtils.GetDefaultMockProvisioningService(null, "DefaultMasterSecret", null);

            var connectionService = new DefaultConnectionService(
                _eventAggregator,
                new DefaultWalletRecordService(),
                provisioningService,
                new Mock<ILogger<DefaultConnectionService>>().Object);

            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () => await connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration()));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task CanCreateRequestWithoutEndpoint()
        {
            var provisioningService = ServiceUtils.GetDefaultMockProvisioningService(null, "DefaultMasterSecret", null);

            var connectionService = new DefaultConnectionService(
                _eventAggregator,
                new DefaultWalletRecordService(),
                provisioningService,
                new Mock<ILogger<DefaultConnectionService>>().Object);

            var (invite, _) = await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration());

            var (request, _) = await connectionService.CreateRequestAsync(_holderWallet, invite);

            Assert.True(request.Connection.DidDoc.Services.Count == 0);
        }

        [Fact]
        public async Task CanRecieveRequestWithoutEndpoint()
        {
            var provisioningService = ServiceUtils.GetDefaultMockProvisioningService(null, "DefaultMasterSecret", null);

            var connectionService = new DefaultConnectionService(
                _eventAggregator,
                new DefaultWalletRecordService(),
                provisioningService,
                new Mock<ILogger<DefaultConnectionService>>().Object);

            var (invite, inviteeConnection) = await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration());

            var (request, _) = await connectionService.CreateRequestAsync(_holderWallet, invite);

            var id = await _connectionService.ProcessRequestAsync(_issuerWallet, request, inviteeConnection);

            inviteeConnection = await _connectionService.GetAsync(_issuerWallet, id);

            Assert.True(inviteeConnection.State == ConnectionState.Negotiating);
            Assert.True(request.Connection.DidDoc.Services.Count == 0);
        }

        [Fact]
        public async Task AcceptRequestThrowsExceptionConnectionNotFound()
        {
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () => await _connectionService.CreateResponseAsync(_issuerWallet, "bad-connection-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task AcceptRequestThrowsExceptionConnectionInvalidState()
        {
            var connectionId = Guid.NewGuid().ToString();
            
            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId, AutoAcceptConnection = false });

            //Process a connection request
            var connectionRecord = await _connectionService.GetAsync(_issuerWallet, connectionId);

            await _connectionService.ProcessRequestAsync(_issuerWallet, new ConnectionRequestMessage
            {
                Connection = new Connection {
                    Did = "EYS94e95kf6LXF49eARL76",
                    DidDoc = new ConnectionRecord
                    {
                        MyVk = "6vyxuqpe3UBcTmhF3Wmmye2UVroa51Lcd9smQKFB5QX1"
                    }.MyDidDoc(await _provisioningService.GetProvisioningAsync(_issuerWallet.Wallet))
                }
            }, connectionRecord);

            //Accept the connection request
            await _connectionService.CreateResponseAsync(_issuerWallet, connectionId);

            //Now try and accept it again
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () => await _connectionService.CreateResponseAsync(_issuerWallet, connectionId));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task RevokeInvitationThrowsConnectionNotFound()
        {
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () => await _connectionService.RevokeInvitationAsync(_issuerWallet, "bad-connection-id"));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task RevokeInvitationThrowsConnectionInvalidState()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId, AutoAcceptConnection = false });

            //Process a connection request
            var connectionRecord = await _connectionService.GetAsync(_issuerWallet, connectionId);

            await _connectionService.ProcessRequestAsync(_issuerWallet, new ConnectionRequestMessage
            {
                Connection = new Connection { 
                    Did = "EYS94e95kf6LXF49eARL76",
                    DidDoc = new ConnectionRecord
                    {
                        MyVk = "6vyxuqpe3UBcTmhF3Wmmye2UVroa51Lcd9smQKFB5QX1"
                    }.MyDidDoc(await _provisioningService.GetProvisioningAsync(_issuerWallet.Wallet))
                }
            }, connectionRecord);

            //Accept the connection request
            await _connectionService.CreateResponseAsync(_issuerWallet, connectionId);

            //Now try and revoke invitation
            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () => await _connectionService.RevokeInvitationAsync(_issuerWallet, connectionId));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task CanRevokeInvitation()
        {
            var connectionId = Guid.NewGuid().ToString();

            await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { ConnectionId = connectionId });

            var connection = await _connectionService.GetAsync(_issuerWallet, connectionId);

            Assert.False(connection.MultiPartyInvitation);
            Assert.Equal(ConnectionState.Invited, connection.State);
            Assert.Equal(connectionId, connection.Id);

            await _connectionService.RevokeInvitationAsync(_issuerWallet, connectionId);

            var ex = await Assert.ThrowsAsync<AgentFrameworkException>(async () => await _connectionService.CreateResponseAsync(_issuerWallet, connectionId));
            Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task CanEstablishConnectionAsync()
        {
            var events = 0;
            _eventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
                .Where(_ => (_.MessageType == MessageTypes.ConnectionRequest ||
                             _.MessageType == MessageTypes.ConnectionResponse))
                .Subscribe(_ =>
                {
                    events++;
                });


            var (connectionIssuer, connectionHolder) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet);

            Assert.True(events == 2);

            Assert.Equal(ConnectionState.Connected, connectionIssuer.State);
            Assert.Equal(ConnectionState.Connected, connectionHolder.State);

            Assert.Equal(connectionIssuer.MyDid, connectionHolder.TheirDid);
            Assert.Equal(connectionIssuer.TheirDid, connectionHolder.MyDid);

            Assert.Equal(connectionIssuer.Endpoint.Uri, TestConstants.DefaultMockUri);
            Assert.Equal(connectionIssuer.Endpoint.Uri, TestConstants.DefaultMockUri);
        }

        [Fact]
        public async Task CanEstablishConnectionsWithMultiPartyInvitationAsync()
        {
            (var invite, var record) = await _connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration { MultiPartyInvitation = true });

            var (connectionIssuer, connectionHolderOne) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWallet, invite, record.Id);

            _messages.Clear();

            var (connectionIssuerTwo, connectionHolderTwo) = await Scenarios.EstablishConnectionAsync(
                _connectionService, _messages, _issuerWallet, _holderWalletTwo, invite, record.Id);

            Assert.Equal(ConnectionState.Connected, connectionIssuer.State);
            Assert.Equal(ConnectionState.Connected, connectionHolderOne.State);

            Assert.Equal(ConnectionState.Connected, connectionIssuerTwo.State);
            Assert.Equal(ConnectionState.Connected, connectionHolderTwo.State);

            Assert.Equal(connectionIssuer.MyDid, connectionHolderOne.TheirDid);
            Assert.Equal(connectionIssuer.TheirDid, connectionHolderOne.MyDid);

            Assert.Equal(connectionIssuerTwo.MyDid, connectionHolderTwo.TheirDid);
            Assert.Equal(connectionIssuerTwo.TheirDid, connectionHolderTwo.MyDid);

            Assert.Equal(connectionIssuer.Endpoint.Uri, TestConstants.DefaultMockUri);
            Assert.Equal(connectionIssuerTwo.Endpoint.Uri, TestConstants.DefaultMockUri);
        }
        
        public async Task DisposeAsync()
        {
            if (_issuerWallet != null) await _issuerWallet.Wallet.CloseAsync();
            if (_holderWallet != null) await _holderWallet.Wallet.CloseAsync();
            if (_holderWalletTwo != null) await _holderWalletTwo.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(_issuerConfig, Credentials);
            await Wallet.DeleteWalletAsync(_holderConfig, Credentials);
            await Wallet.DeleteWalletAsync(_holderConfigTwo, Credentials);
        }
    }
}