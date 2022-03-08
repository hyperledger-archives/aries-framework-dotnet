using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Features.Handshakes.Common;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class StateMachineTests
    {
        [Fact]
        public async Task CanTransitionFromDisconnetedToNegotiatingWithInvitationAccept()
        {
            var record = new ConnectionRecord();

            Assert.True(ConnectionState.Invited == record.State);

            await record.TriggerAsync(ConnectionTrigger.InvitationAccept);

            Assert.True(ConnectionState.Negotiating == record.State);
        }

        [Fact]
        public async Task CanTransitionFromInvitedToNegotiatingWithRequest()
        {
            var record = new ConnectionRecord { State = ConnectionState.Invited };

            Assert.True(ConnectionState.Invited == record.State);

            await record.TriggerAsync(ConnectionTrigger.Request);

            Assert.True(ConnectionState.Negotiating == record.State);
        }

        [Fact]
        public async Task CanTransitionFromNegotiatingToConnectedWithRespone()
        {
            var record = new ConnectionRecord { State = ConnectionState.Negotiating };

            Assert.True(ConnectionState.Negotiating == record.State);

            await record.TriggerAsync(ConnectionTrigger.Response);

            Assert.True(ConnectionState.Connected == record.State);
        }

        [Fact]
        public async Task CannotTransitionFromNegotiatingToConnectedWithRequest()
        {
            var record = new ConnectionRecord { State = ConnectionState.Negotiating};

            Assert.True(ConnectionState.Negotiating == record.State);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await record.TriggerAsync(ConnectionTrigger.Request)); 

            Assert.True(ConnectionState.Negotiating == record.State);
        }

        [Fact]
        public async Task CannotTransitionFromNegotiatingWithAccept()
        {
            var record = new ConnectionRecord { State = ConnectionState.Negotiating};

            Assert.True(ConnectionState.Negotiating == record.State);

            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => record.TriggerAsync(ConnectionTrigger.InvitationAccept));

            Assert.Equal("Stateless", exception.Source);
        }

        [Theory]
        [InlineData(ConnectionState.Invited)]
        [InlineData(ConnectionState.Negotiating)]
        [InlineData(ConnectionState.Connected)]
        [InlineData(ConnectionState.Abandoned)]
        public async Task CanTransitionToAbandonedFromAllStates(ConnectionState state)
        {
            var record = new ConnectionRecord { State = state};

            Assert.Equal(state, record.State);
            
            await record.TriggerAsync(ConnectionTrigger.Abandon);

            Assert.Equal(ConnectionState.Abandoned, record.State);
        }
        
        [Theory]
        [InlineData(ConnectionTrigger.InvitationAccept)]
        [InlineData(ConnectionTrigger.Request)]
        [InlineData(ConnectionTrigger.Response)]
        [InlineData(ConnectionTrigger.Complete)]
        public async Task CannotTransitionFromAbandonedToAnotherState(ConnectionTrigger trigger)
        {
            var record = new ConnectionRecord { State = ConnectionState.Abandoned};

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await record.TriggerAsync(trigger));
            
            Assert.Equal(ConnectionState.Abandoned, record.State);
        }

        [Theory]
        [InlineData(ConnectionTrigger.Response)]
        [InlineData(ConnectionTrigger.Complete)]
        public async Task OnlyRequestAllowsTransitionFromInvitedToNegotiating(ConnectionTrigger trigger)
        {
            var record = new ConnectionRecord();
            
            Assert.Equal(ConnectionState.Invited, record.State);
            
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await record.TriggerAsync(trigger));
            
            Assert.Equal(ConnectionState.Invited, record.State);
        }
        
        [Theory]
        [InlineData(ConnectionTrigger.Request)]
        [InlineData(ConnectionTrigger.Complete)]
        public async Task OnlyRespondAllowsTransitionFromNegotiatingToConnected(ConnectionTrigger trigger)
        {
            var record = new ConnectionRecord() {State = ConnectionState.Negotiating};
            
            Assert.Equal(ConnectionState.Negotiating, record.State);
            
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await record.TriggerAsync(trigger));
            
            Assert.Equal(ConnectionState.Negotiating, record.State);
        }
    }
}