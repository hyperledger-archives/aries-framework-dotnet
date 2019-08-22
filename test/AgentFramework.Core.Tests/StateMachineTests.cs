using System;
using System.Threading.Tasks;
using AgentFramework.Core.Models.Records;
using Xunit;

namespace AgentFramework.Core.Tests
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
        public async Task CanTransitionFromNegotiatingToConnectedWithRequest()
        {
            var record = new ConnectionRecord { State = ConnectionState.Negotiating};

            Assert.True(ConnectionState.Negotiating == record.State);

            await record.TriggerAsync(ConnectionTrigger.Request);

            Assert.True(ConnectionState.Connected == record.State);
        }

        [Fact]
        public async Task CannotTransitionFromInvitedWithAccept()
        {
            var record = new ConnectionRecord { State = ConnectionState.Negotiating};

            Assert.True(ConnectionState.Negotiating == record.State);

            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    () => record.TriggerAsync(ConnectionTrigger.InvitationAccept));

            Assert.Equal("Stateless", exception.Source);
        }
    }
}