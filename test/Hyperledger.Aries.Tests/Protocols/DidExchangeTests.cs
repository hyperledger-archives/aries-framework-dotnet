using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Ledger;
using Hyperledger.Aries.Runtime;
using Hyperledger.Aries.Storage;
using Hyperledger.Indy.WalletApi;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Utils;
using Moq;
using Xunit;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class DidExchangeTests : IAsyncLifetime
    {
        private const string NYM_RESULT = @"{""op"":""REPLY"",""result"":{""state_proof"":{""multi_signature"":{""participants"":[""Node2"",""Node4"",""Node3""],""signature"":""RB78hAjiYetVe29QwJJwixzVhJjY63r7WMADudjWXQ2qaYaQ9ufnNdoN8oThvu2sJEM5MYiVRbFkzzE9PnX5jTn7R4C1tYx24D5tLVuNagmFzuEbwsNcAAc1aAHHiqsu1pbFRqAenZDRNfkUZTuqNEHXvZBumyno8LjoQf4twnRFVZ"",""value"":{""timestamp"":1644253560,""ledger_id"":1,""pool_state_root_hash"":""NCGqbfRWDWtLB2bDuL6TC5BhrRdQMc5MyKdXQqXii44"",""txn_root_hash"":""HgxLc6h6R12P5zSM1h7T5jSttmAoHryhmrshffmM9imx"",""state_root_hash"":""39aVhSB3idDKLkKTUGfKHFMhE2oNZ5GyCSDqCg4M5x2U""}},""proof_nodes"":""+QH++FGgzY5nre+zKgvHxP7nkWuZxqoZnAaToNDoVR9rpa+Pe5eAgICAgICAgKCNWfBDnYxBU3V8cL2sqWAG1rg+zEh1PDwuaUeXaXlOBYCAgICAgID4laAgvt1knpas6UtCrrsAVhgRtc4VajkihVGY969PtfLyd7hy+HC4bnsiaWRlbnRpZmllciI6IlY0U0dSVTg2WjU4ZDZUVjdQQlVlNmYiLCJyb2xlIjoiMiIsInNlcU5vIjoyLCJ0eG5UaW1lIjpudWxsLCJ2ZXJrZXkiOiJ+N1RZZmVrdzRHVWFnQm5CVkNxUGppQyJ9+QERoNPSP24JsVps7QufK62cHm4MLrVBpYu1VMlThcJrixajgICgmpq6PvRB/76zSDjdvXO+dATJAmHaV82rEVG2ZoAO+TCAoAIbx/TDY2y4OJtZiJtzVNjJICBQpJ4h68cXrBVl0wEvoEQAggmzS6f2NWpUQAFsJZORzSvJtNWsWBopTosPpyIZgICAgKAkcibehQ5iUOtCXD3lQORF05za0YiwT2DavXYCOkmJQoCgfSVcTjtW1SUW/CcILmuBjzvybpOl4RlMkgp3hJTFe2qgG9KhOW51cPReSFM+bhjF8JwB0JqIO+TMzzDo7i/n97WgOzoZfFEvyKcPZvdFjW/+5+SwMc2p8UporYwQiM0hnBqA"",""root_hash"":""39aVhSB3idDKLkKTUGfKHFMhE2oNZ5GyCSDqCg4M5x2U""},""type"":""105"",""seqNo"":2,""txnTime"":null,""reqId"":1644253614722541000,""dest"":""Th7MpTaRZVRYnPiabds81Y"",""data"":""{""dest"":""Th7MpTaRZVRYnPiabds81Y"",""identifier"":""V4SGRU86Z58d6TV7PBUe6f"",""role"":""2"",""seqNo"":2,""txnTime"":null,""verkey"":""~7TYfekw4GUagBnBVCqPjiC""}"",""identifier"":""LibindyDid111111111111""}}";
        private const string ENDPOINT_ATTRIBUTE_RESULT = "{\"result\":{\"txnTime\":1644258368,\"type\":\"104\",\"identifier\":\"LibindyDid111111111111\",\"state_proof\":{\"root_hash\":\"78GPASak7XZGhwSnygRTXp3DWSSmSsDYjJmM2Qix69h8\",\"multi_signature\":{\"value\":{\"state_root_hash\":\"78GPASak7XZGhwSnygRTXp3DWSSmSsDYjJmM2Qix69h8\",\"txn_root_hash\":\"83nqtLD2KJxC92pbh2BBkZDTGHSjMhwo5Z7w6vD1ntXs\",\"ledger_id\":1,\"timestamp\":1644258368,\"pool_state_root_hash\":\"NCGqbfRWDWtLB2bDuL6TC5BhrRdQMc5MyKdXQqXii44\"},\"participants\":[\"Node4\",\"Node3\",\"Node1\"],\"signature\":\"Qv6EYdw7hdNYijaf6eWnzk7ddpwti7XhinjNFmyXHpBDP7pDc6b93Q5FZMMx7dseWDhs19YEBfgXHbjgw3zSqhWJL6YduGvWkNkeakAhRPmXdYSh6BUzqjXigKrHYj45N2GDuFviuAfddMZdjKhAwsCwY8P5VWLiy9DuFBVoqQc6Nb\"},\"proof_nodes\":\"+QIu+MW4WSBoN01wVGFSWlZSWW5QaWFiZHM4MVk6MTpiNmJmN2JjOGQ5NmYzZWE5ZDEzMmM4M2IzZGE4ZTc3NjBlNDIwMTM4NDg1NjU3MzcyZGI0ZDZhOTgxZDNmZDlluGj4ZrhkeyJsc24iOjE1LCJsdXQiOjE2NDQyNTgzNjgsInZhbCI6IjllMDc2MTkxMzljYTNjN2RmMWUyODM4YzBjYmM0MjE2NzE1MzRiMjUyMDM2YzE3ZDQ4Nzg2NTQyY2E4ZWU0YmMiffhRgICAgKAOlUkahUWDOikVdUeL5IS5Ufj1R6uaP5egHJMdVKKOdYCAgICglXmtsWZRBdVkJTAbJG8Y4ZSVxrW+fD8LoNGVEk8I6qSAgICAgICA+QERoNPSP24JsVps7QufK62cHm4MLrVBpYu1VMlThcJrixajgICgmpq6PvRB/76zSDjdvXO+dATJAmHaV82rEVG2ZoAO+TCAoD66VgPCPNiQ2fYUKtVdQODEKXwBdq4vRg7XVBGtKFvhoEQAggmzS6f2NWpUQAFsJZORzSvJtNWsWBopTosPpyIZgICAgKAkcibehQ5iUOtCXD3lQORF05za0YiwT2DavXYCOkmJQoCgfSVcTjtW1SUW/CcILmuBjzvybpOl4RlMkgp3hJTFe2qgG9KhOW51cPReSFM+bhjF8JwB0JqIO+TMzzDo7i/n97WgOzoZfFEvyKcPZvdFjW/+5+SwMc2p8UporYwQiM0hnBqA\"},\"dest\":\"Th7MpTaRZVRYnPiabds81Y\",\"raw\":\"endpoint\",\"seqNo\":15,\"reqId\":1644258372930687000,\"data\":\"{\"endpoint\":{\"endpoint\":\"http://test\"}}\"},\"op\":\"REPLY\"}";
        
        private readonly string _responderConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private readonly string _requesterConfig = $"{{\"id\":\"{Guid.NewGuid()}\"}}";
        private const string Credentials = "{\"key\":\"test_wallet_key\"}";

        private IAgentContext _responder;
        private IAgentContext _requester;

        private readonly IDidExchangeService _didExchangeService;

        public DidExchangeTests()
        {
            IEventAggregator eventAggregator = new EventAggregator();
            var provisioningService = ServiceUtils.GetDefaultMockProvisioningService();
            var ledgerService = new DefaultLedgerService(new Mock<ILedgerSigningService>().Object);
            _didExchangeService = new DefaultDidExchangeService(ledgerService, new Mock<IWalletRecordService>().Object,
                provisioningService, eventAggregator);
        }

        public async Task InitializeAsync()
        {
            _responder = await AgentUtils.Create(_responderConfig, Credentials);
            _requester = await AgentUtils.Create(_requesterConfig, Credentials, true);
        }

        [Fact]
        public async Task CanCreateRequestWithImplicitInvitation()
        {
            var (request, _) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);

            Assert.NotNull(request);
        }

        [Fact]
        public async Task CanProcessRequest()
        {
            var (request, _) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var inviteeConnection = await _didExchangeService.ProcessRequestAsync(_responder, request);

            Assert.True(inviteeConnection.State == ConnectionState.Negotiating);
        }

        [Fact]
        public async Task CanCreateResponse()
        {
            var (request, _) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var responderRecord = await _didExchangeService.ProcessRequestAsync(_responder, request);

            var (response, record) = await _didExchangeService.CreateResponseAsync(_responder, responderRecord);
            
            Assert.NotNull(response);
            Assert.Equal(ConnectionState.Connected, record.State);
        }

        [Fact]
        public async Task CanProcessResponse()
        {
            var (request, requesterRecord) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var responderRecord = await _didExchangeService.ProcessRequestAsync(_responder, request);

            var (response, _) = await _didExchangeService.CreateResponseAsync(_responder, responderRecord);
            
            requesterRecord = await _didExchangeService.ProcessResponseAsync(_requester, response, requesterRecord);
            
            Assert.Equal(ConnectionState.Connected, requesterRecord.State);
        }
        
        [Fact]
        public async Task CanCreateComplete()
        {
            var (request, requesterRecord) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var responderRecord = await _didExchangeService.ProcessRequestAsync(_responder, request);

            var (response, _) = await _didExchangeService.CreateResponseAsync(_responder, responderRecord);
            
            requesterRecord = await _didExchangeService.ProcessResponseAsync(_requester, response, requesterRecord);

            var (completeMessage, completeRecord) = await _didExchangeService.CreateComplete(_requester, requesterRecord);
            
            Assert.NotNull(completeMessage);
            Assert.Equal(ConnectionState.Connected, completeRecord.State);
        }
        
        [Fact]
        public async Task CanProcessComplete()
        {
            var (request, requesterRecord) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var responderRecord = await _didExchangeService.ProcessRequestAsync(_responder, request);

            var (response, responseRecord) = await _didExchangeService.CreateResponseAsync(_responder, responderRecord);
            
            requesterRecord = await _didExchangeService.ProcessResponseAsync(_requester, response, requesterRecord);

            var (completeMessage, completeRecord) = await _didExchangeService.CreateComplete(_requester, requesterRecord);
            
            responderRecord = await _didExchangeService.ProcessComplete(_responder, completeMessage, responseRecord);
            
            Assert.Equal(ConnectionState.Connected, responderRecord.State);
        }

        [Fact]
        public async Task CanCreateProblemReport()
        {
            var (request, requesterRecord) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var responderRecord = await _didExchangeService.ProcessRequestAsync(_responder, request);
            
            var (problemReportMessage, responseRecord) = await _didExchangeService.AbandonDidExchange(_responder, responderRecord);
            
            Assert.NotNull(problemReportMessage);
            Assert.Equal(ConnectionState.Abandoned, responseRecord.State);
        }
        
        [Fact]
        public async Task CanProcessProblemReport()
        {
            var (request, requesterRecord) = await _didExchangeService.CreateRequestAsync(_requester, TestConstants.StewardDid);
            
            var responderRecord = await _didExchangeService.ProcessRequestAsync(_responder, request);
            
            var (problemReportMessage, responseRecord) = await _didExchangeService.AbandonDidExchange(_responder, responderRecord);
            
            requesterRecord = await _didExchangeService.ProcessProblemReportMessage(_requester, problemReportMessage, requesterRecord);
            
            Assert.Equal(ConnectionState.Abandoned, requesterRecord.State);
        }

        public async Task DisposeAsync()
        {
            if (_responder != null) await _responder.Wallet.CloseAsync();
            if (_requester != null) await _requester.Wallet.CloseAsync();

            await Wallet.DeleteWalletAsync(_responderConfig, Credentials);
            await Wallet.DeleteWalletAsync(_requesterConfig, Credentials);
        }
    }
}