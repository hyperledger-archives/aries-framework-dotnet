using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection;
using Hyperledger.Aries.Features.Handshakes.DidExchange;
using Hyperledger.Aries.Models.Events;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.Utils;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hyperledger.Aries.Tests.Integration
{
    public class DidExchangeTests : IAsyncLifetime
    {
        static DidExchangeTests()
        {
            global::Hyperledger.Aries.Utils.Runtime.SetFlags(Hyperledger.Aries.Utils.Runtime.LedgerLookupRetryFlag);
        }
        
        WalletConfiguration config1 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config2 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletCredentials cred = new WalletCredentials { Key = "2" };

        private MockAgent _responder;
        private MockAgent _requester;
        private readonly MockAgentRouter _router = new MockAgentRouter();

        public async Task InitializeAsync()
        {
            _responder = await MockUtils.CreateAsync("responder", config1, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), TestConstants.StewardSeed);
            _router.RegisterAgent(_responder);
            _requester = await MockUtils.CreateAsync("requester", config2, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)));
            _router.RegisterAgent(_requester);
            
            var ledgerService = _responder.ServiceProvider.GetRequiredService<ILedgerService>();
            await ledgerService.RegisterServiceEndpointAsync(_responder.Context, TestConstants.StewardDid,
                "http://responder");
        }

        [Fact]
        public async Task CanExchangeDid()
        {
            var slim = new SemaphoreSlim(0, 1);
            
            var didExchangeService = _requester.GetService<IDidExchangeService>();
            var connectionService = _requester.GetService<IConnectionService>();
            var messsageService = _requester.GetService<IMessageService>();

            // Did Exchange Request
            _responder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Request)
                .Subscribe(x => slim.Release());

            (var request, var requesterConnection) = await didExchangeService.CreateRequestAsync(_requester.Context, TestConstants.StewardDid);

            await messsageService.SendAsync(_requester.Context, request, requesterConnection);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));

            var requesterRecord = await connectionService.GetAsync(_requester.Context, requesterConnection.Id);
            var responderRecord = (await connectionService.ListAsync(_responder.Context, SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), requesterConnection.MyDid))).First();

            Assert.Equal(ConnectionState.Negotiating, responderRecord.State);
            Assert.Equal(ConnectionState.Negotiating, requesterRecord.State);
            Assert.Equal(requesterRecord.TheirDid, TestConstants.StewardDid);
            Assert.Equal(responderRecord.TheirDid, requesterRecord.MyDid);

            Assert.Equal(
                requesterRecord.GetTag(TagConstants.LastThreadId),
                responderRecord.GetTag(TagConstants.LastThreadId));
            
            // Did Exchange Response
            _requester.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Response)
                .Subscribe(x => slim.Release());
            
            var (response, newResponderRecord) = await didExchangeService.CreateResponseAsync(_responder.Context, responderRecord);
            
            await messsageService.SendAsync(_responder.Context, response, newResponderRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            var newRequesterRecord = (await connectionService.ListAsync(_requester.Context, SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), newResponderRecord.MyDid))).First();
            
            Assert.Equal(ConnectionState.Connected, newResponderRecord.State);
            Assert.Equal(ConnectionState.Connected, newRequesterRecord.State);
            Assert.Equal(newRequesterRecord.TheirDid, newResponderRecord.MyDid);
            Assert.Equal(newResponderRecord.TheirDid, newRequesterRecord.MyDid);

            Assert.Equal(
                newRequesterRecord.GetTag(TagConstants.LastThreadId),
                newResponderRecord.GetTag(TagConstants.LastThreadId));
            
            // Did Exchange Complete
            _responder.GetService<IEventAggregator>().GetEventByType<ServiceMessageProcessingEvent>()
                .Where(x => x.MessageType == MessageTypesHttps.DidExchange.Complete)
                .Subscribe(x => slim.Release());

            var (completeMessage, finalRequesterRecord) = await didExchangeService.CreateComplete(_requester.Context, newRequesterRecord);

            await messsageService.SendAsync(_requester.Context, completeMessage, finalRequesterRecord);
            
            await slim.WaitAsync(TimeSpan.FromSeconds(30));
            
            var finalResponderRecord = (await connectionService.ListAsync(_responder.Context, SearchQuery.Equal(nameof(ConnectionRecord.TheirDid), finalRequesterRecord.MyDid))).First();
            
            Assert.Equal(ConnectionState.Connected, finalResponderRecord.State);
            Assert.Equal(ConnectionState.Connected, finalRequesterRecord.State);
            Assert.Equal(finalRequesterRecord.TheirDid, finalResponderRecord.MyDid);
            Assert.Equal(finalResponderRecord.TheirDid, finalRequesterRecord.MyDid);
            
            Assert.Equal(
                finalRequesterRecord.GetTag(TagConstants.LastThreadId),
                finalResponderRecord.GetTag(TagConstants.LastThreadId));
        }

        public async Task DisposeAsync()
        {
            await _responder.Dispose();
            await _requester.Dispose();
        }
    }
}
