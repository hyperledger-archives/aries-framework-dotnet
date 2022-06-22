using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Decorators.Threading;
using Hyperledger.Aries.Features.Handshakes;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Features.Handshakes.Connection.Models;
using Hyperledger.Aries.Features.OutOfBand;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries.TestHarness;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hyperledger.Aries.Tests.Integration
{
    public class OutOfBandTests : IAsyncLifetime
    {
        WalletConfiguration config1 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletConfiguration config2 = new WalletConfiguration { Id = Guid.NewGuid().ToString() };
        WalletCredentials cred = new WalletCredentials { Key = "2" };

        private MockAgent _sender;
        private MockAgent _receiver;
        private readonly MockAgentRouter _router = new MockAgentRouter();
        
        public async Task InitializeAsync()
        {
            _sender = await MockUtils.CreateAsync("sender", config1, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)), TestConstants.StewardSeed);
            _router.RegisterAgent(_sender);
            _receiver = await MockUtils.CreateAsync("receiver", config2, cred, new MockAgentHttpHandler((cb) => _router.RouteMessage(cb.name, cb.data)));
            _router.RegisterAgent(_receiver);
            
            var ledgerService = _sender.ServiceProvider.GetRequiredService<ILedgerService>();
            await ledgerService.RegisterServiceEndpointAsync(_sender.Context, TestConstants.StewardDid,
                "http://sender");
        }

        [Fact]
        public async Task CanDoHandshakeIfNotConnected()
        {
            var (receiverRecord, senderRecord) = await AgentScenarios.ExchangeDidsWithPrivateDids(_receiver, _sender); 
            
            Assert.NotNull(receiverRecord);
            Assert.NotNull(senderRecord);
            Assert.Equal(ConnectionState.Connected, receiverRecord.State);
            Assert.Equal(ConnectionState.Connected, senderRecord.State);
        }

        [Fact]
        public Task CanAnswerRequestWithoutConnection()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public Task CanAnswerRequestAfterCreatingNewConnection()
        {
            return Task.CompletedTask;
        }
        
        [Fact]
        public Task CanAnswerRequestWithExistingConnection()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CanReuseHandshakeIfAlreadyConnected()
        {
            var outOfBandService = _sender.GetService<IOutOfBandService>();
            var messageService = _receiver.GetService<IMessageService>();

            var (receiverRecord, senderRecord) = await AgentScenarios.ExchangeDidsWithPublicDid(_receiver, _sender);

            var (invitation, _) = await outOfBandService.CreateInvitationAsync(_sender.Context, null, new InviteConfiguration {UsePublicDid = true});
            
            var (msg, record) = await outOfBandService.ProcessInvitationMessage(_receiver.Context, invitation);
            
            Assert.NotNull(msg);
            Assert.NotNull(record);
            Assert.Equal(HandshakeProtocol.DidExchange, record.HandshakeProtocol);
            Assert.Equal(ConnectionState.Connected, record.State);
            Assert.Equal(msg.Id, msg.GetThreadId());
            Assert.Equal(invitation.GetThreadId(), msg.GetParentThreadId());

            var reuseAcceptedMessage = await messageService.SendReceiveAsync<HandshakeReuseAcceptedMessage>(_receiver.Context, msg, record);

            Assert.NotNull(reuseAcceptedMessage);
            Assert.Equal(msg.GetThreadId(), reuseAcceptedMessage.GetThreadId());
            Assert.Equal(invitation.Id, reuseAcceptedMessage.GetParentThreadId());
            
            await outOfBandService.ProcessHandshakeReuseAccepted(_receiver.Context, reuseAcceptedMessage);
        }

        public async Task DisposeAsync()
        {
            await _sender.Dispose();
            await _receiver.Dispose();
        }
    }
}
