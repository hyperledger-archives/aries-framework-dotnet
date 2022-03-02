using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Runtime;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Hyperledger.TestHarness.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class DidExchangeTests : IAsyncLifetime
    {
        private const string NYM_RESULT =
            @"{""op"":""REPLY"",""result"":{""state_proof"":{""multi_signature"":{""participants"":[""Node2"",""Node4"",""Node3""],""signature"":""RB78hAjiYetVe29QwJJwixzVhJjY63r7WMADudjWXQ2qaYaQ9ufnNdoN8oThvu2sJEM5MYiVRbFkzzE9PnX5jTn7R4C1tYx24D5tLVuNagmFzuEbwsNcAAc1aAHHiqsu1pbFRqAenZDRNfkUZTuqNEHXvZBumyno8LjoQf4twnRFVZ"",""value"":{""timestamp"":1644253560,""ledger_id"":1,""pool_state_root_hash"":""NCGqbfRWDWtLB2bDuL6TC5BhrRdQMc5MyKdXQqXii44"",""txn_root_hash"":""HgxLc6h6R12P5zSM1h7T5jSttmAoHryhmrshffmM9imx"",""state_root_hash"":""39aVhSB3idDKLkKTUGfKHFMhE2oNZ5GyCSDqCg4M5x2U""}},""proof_nodes"":""+QH++FGgzY5nre+zKgvHxP7nkWuZxqoZnAaToNDoVR9rpa+Pe5eAgICAgICAgKCNWfBDnYxBU3V8cL2sqWAG1rg+zEh1PDwuaUeXaXlOBYCAgICAgID4laAgvt1knpas6UtCrrsAVhgRtc4VajkihVGY969PtfLyd7hy+HC4bnsiaWRlbnRpZmllciI6IlY0U0dSVTg2WjU4ZDZUVjdQQlVlNmYiLCJyb2xlIjoiMiIsInNlcU5vIjoyLCJ0eG5UaW1lIjpudWxsLCJ2ZXJrZXkiOiJ+N1RZZmVrdzRHVWFnQm5CVkNxUGppQyJ9+QERoNPSP24JsVps7QufK62cHm4MLrVBpYu1VMlThcJrixajgICgmpq6PvRB/76zSDjdvXO+dATJAmHaV82rEVG2ZoAO+TCAoAIbx/TDY2y4OJtZiJtzVNjJICBQpJ4h68cXrBVl0wEvoEQAggmzS6f2NWpUQAFsJZORzSvJtNWsWBopTosPpyIZgICAgKAkcibehQ5iUOtCXD3lQORF05za0YiwT2DavXYCOkmJQoCgfSVcTjtW1SUW/CcILmuBjzvybpOl4RlMkgp3hJTFe2qgG9KhOW51cPReSFM+bhjF8JwB0JqIO+TMzzDo7i/n97WgOzoZfFEvyKcPZvdFjW/+5+SwMc2p8UporYwQiM0hnBqA"",""root_hash"":""39aVhSB3idDKLkKTUGfKHFMhE2oNZ5GyCSDqCg4M5x2U""},""type"":""105"",""seqNo"":2,""txnTime"":null,""reqId"":1644253614722541000,""dest"":""Th7MpTaRZVRYnPiabds81Y"",""data"":""{""dest"":""Th7MpTaRZVRYnPiabds81Y"",""identifier"":""V4SGRU86Z58d6TV7PBUe6f"",""role"":""2"",""seqNo"":2,""txnTime"":null,""verkey"":""~7TYfekw4GUagBnBVCqPjiC""}"",""identifier"":""LibindyDid111111111111""}}";
        
        private const string ENDPOINT_ATTRIBUTE_RESULT = "{\"result\":{\"txnTime\":1644258368,\"type\":\"104\",\"identifier\":\"LibindyDid111111111111\",\"state_proof\":{\"root_hash\":\"78GPASak7XZGhwSnygRTXp3DWSSmSsDYjJmM2Qix69h8\",\"multi_signature\":{\"value\":{\"state_root_hash\":\"78GPASak7XZGhwSnygRTXp3DWSSmSsDYjJmM2Qix69h8\",\"txn_root_hash\":\"83nqtLD2KJxC92pbh2BBkZDTGHSjMhwo5Z7w6vD1ntXs\",\"ledger_id\":1,\"timestamp\":1644258368,\"pool_state_root_hash\":\"NCGqbfRWDWtLB2bDuL6TC5BhrRdQMc5MyKdXQqXii44\"},\"participants\":[\"Node4\",\"Node3\",\"Node1\"],\"signature\":\"Qv6EYdw7hdNYijaf6eWnzk7ddpwti7XhinjNFmyXHpBDP7pDc6b93Q5FZMMx7dseWDhs19YEBfgXHbjgw3zSqhWJL6YduGvWkNkeakAhRPmXdYSh6BUzqjXigKrHYj45N2GDuFviuAfddMZdjKhAwsCwY8P5VWLiy9DuFBVoqQc6Nb\"},\"proof_nodes\":\"+QIu+MW4WSBoN01wVGFSWlZSWW5QaWFiZHM4MVk6MTpiNmJmN2JjOGQ5NmYzZWE5ZDEzMmM4M2IzZGE4ZTc3NjBlNDIwMTM4NDg1NjU3MzcyZGI0ZDZhOTgxZDNmZDlluGj4ZrhkeyJsc24iOjE1LCJsdXQiOjE2NDQyNTgzNjgsInZhbCI6IjllMDc2MTkxMzljYTNjN2RmMWUyODM4YzBjYmM0MjE2NzE1MzRiMjUyMDM2YzE3ZDQ4Nzg2NTQyY2E4ZWU0YmMiffhRgICAgKAOlUkahUWDOikVdUeL5IS5Ufj1R6uaP5egHJMdVKKOdYCAgICglXmtsWZRBdVkJTAbJG8Y4ZSVxrW+fD8LoNGVEk8I6qSAgICAgICA+QERoNPSP24JsVps7QufK62cHm4MLrVBpYu1VMlThcJrixajgICgmpq6PvRB/76zSDjdvXO+dATJAmHaV82rEVG2ZoAO+TCAoD66VgPCPNiQ2fYUKtVdQODEKXwBdq4vRg7XVBGtKFvhoEQAggmzS6f2NWpUQAFsJZORzSvJtNWsWBopTosPpyIZgICAgKAkcibehQ5iUOtCXD3lQORF05za0YiwT2DavXYCOkmJQoCgfSVcTjtW1SUW/CcILmuBjzvybpOl4RlMkgp3hJTFe2qgG9KhOW51cPReSFM+bhjF8JwB0JqIO+TMzzDo7i/n97WgOzoZfFEvyKcPZvdFjW/+5+SwMc2p8UporYwQiM0hnBqA\"},\"dest\":\"Th7MpTaRZVRYnPiabds81Y\",\"raw\":\"endpoint\",\"seqNo\":15,\"reqId\":1644258372930687000,\"data\":\"{\"endpoint\":{\"endpoint\":\"http://test\"}}\"},\"op\":\"REPLY\"}";
        
        private readonly string _issuerConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _holderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _holderConfigTwo = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private IAgentContext _issuerWallet;
        private IAgentContext _holderWallet;
        private IAgentContext _holderWalletTwo;

        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectionService _connectionService;
        private readonly IDidExchangeService _didExchangeService;
        private readonly IProvisioningService _provisioningService;

        private readonly ConcurrentBag<AgentMessage> _messages = new ConcurrentBag<AgentMessage>();

        public DidExchangeTests()
        {
            _eventAggregator = new EventAggregator();
            _provisioningService = ServiceUtils.GetDefaultMockProvisioningService();
            _connectionService = new DefaultConnectionService(
                _eventAggregator,
                new DefaultWalletRecordService(),
                _provisioningService,
                new Mock<ILogger<DefaultConnectionService>>().Object);
            _didExchangeService = new Mock<IDidExchangeService>().Object; // Bookmark: Replace mock
        }

        public async Task InitializeAsync()
        {
            _issuerWallet = await AgentUtils.Create(_issuerConfig, Credentials);
            _holderWallet = await AgentUtils.Create(_holderConfig, Credentials);
            _holderWalletTwo = await AgentUtils.Create(_holderConfigTwo, Credentials);
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

            var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await connectionService.CreateInvitationAsync(_issuerWallet,
                new InviteConfiguration()));

            Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
        }

        [Fact]
        public async Task CanCreateRequestWithImplicitInvitation()
        {
            var provisioningService = ServiceUtils.GetDefaultMockProvisioningService(null, "DefaultMasterSecret", null);
            
            //var didExchangeService = new DefaultDidExchangeService(); // Bookmark: Instantiate didExchangeService here
            
            var (request, _) = await _didExchangeService.CreateRequestAsync(_holderWallet, "did:sov:Th7MpTaRZVRYnPiabds81Y");

            Assert.NotNull(request);
        }
        
        [Fact]
        public async Task CanCreateRequestWithExplicitInvitation()
        {
            // var provisioningService = ServiceUtils.GetDefaultMockProvisioningService(null, "DefaultMasterSecret", null);
            //
            // var connectionService = new DefaultConnectionService(
            //     _eventAggregator,
            //     new DefaultWalletRecordService(),
            //     provisioningService,
            //     new Mock<ILogger<DefaultConnectionService>>().Object);
            //
            // var (invite, _) = await _connectionService.CreateInvitationAsync(_issuerWallet,
            //     new InviteConfiguration());
            //
            // var (request, _) = await connectionService.CreateRequestAsync(_holderWallet, invite);
            //
            // Assert.True(request.Connection.DidDoc.Services.Count == 0);
        }

        [Fact]
        public async Task CanProcessRequest()
        {
            // var provisioningService = ServiceUtils.GetDefaultMockProvisioningService(null, "DefaultMasterSecret", null);
            //
            // var connectionService = new DefaultConnectionService(
            //     _eventAggregator,
            //     new DefaultWalletRecordService(),
            //     provisioningService,
            //     new Mock<ILogger<DefaultConnectionService>>().Object);
            //
            // var (invite, inviteeConnection) = await _connectionService.CreateInvitationAsync(_issuerWallet,
            //     new InviteConfiguration());
            //
            // var (request, _) = await connectionService.CreateRequestAsync(_holderWallet, invite);
            //
            // var id = await _connectionService.ProcessRequestAsync(_issuerWallet, request, inviteeConnection);
            //
            // inviteeConnection = await _connectionService.GetAsync(_issuerWallet, id);
            //
            // Assert.True(inviteeConnection.State == ConnectionState.Negotiating);
            // Assert.True(request.Connection.DidDoc.Services.Count == 0);
        }

        [Fact]
        public async Task CanCreateResponse()
        {
            // var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _connectionService.CreateResponseAsync(_issuerWallet, "bad-connection-id"));
            // Assert.True(ex.ErrorCode == ErrorCode.RecordNotFound);
        }

        [Fact]
        public async Task CanProcessResponse()
        {
            // var connectionId = Guid.NewGuid().ToString();
            //
            // await _connectionService.CreateInvitationAsync(_issuerWallet,
            //     new InviteConfiguration { ConnectionId = connectionId, AutoAcceptConnection = false });
            //
            // //Process a connection request
            // var connectionRecord = await _connectionService.GetAsync(_issuerWallet, connectionId);
            //
            // await _connectionService.ProcessRequestAsync(_issuerWallet, new ConnectionRequestMessage
            // {
            //     Connection = new Connection
            //     {
            //         Did = "EYS94e95kf6LXF49eARL76",
            //         DidDoc = new ConnectionRecord
            //         {
            //             MyVk = "6vyxuqpe3UBcTmhF3Wmmye2UVroa51Lcd9smQKFB5QX1"
            //         }.MyDidDoc(await _provisioningService.GetProvisioningAsync(_issuerWallet.Wallet))
            //     }
            // }, connectionRecord);
            //
            // //Accept the connection request
            // await _connectionService.CreateResponseAsync(_issuerWallet, connectionId);
            //
            // //Now try and accept it again
            // var ex = await Assert.ThrowsAsync<AriesFrameworkException>(async () => await _connectionService.CreateResponseAsync(_issuerWallet, connectionId));
            //
            // Assert.True(ex.ErrorCode == ErrorCode.RecordInInvalidState);
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

            // Bookmark: Replace scenario
            // var (connectionIssuer, connectionHolder) = await Scenarios.EstablishConnectionAsync(
            //     _connectionService, _messages, _issuerWallet, _holderWallet, useDidKeyFormat: useDidKeyFormat);

            // Assert.True(events == 2);
            //
            // Assert.Equal(ConnectionState.Connected, connectionIssuer.State);
            // Assert.Equal(ConnectionState.Connected, connectionHolder.State);
            //
            // Assert.Equal(connectionIssuer.MyDid, connectionHolder.TheirDid);
            // Assert.Equal(connectionIssuer.TheirDid, connectionHolder.MyDid);
            //
            // Assert.Equal(connectionIssuer.Endpoint.Uri, TestConstants.DefaultMockUri);
            // Assert.Equal(connectionIssuer.Endpoint.Uri, TestConstants.DefaultMockUri);
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