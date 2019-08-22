using System;
using AgentFramework.Core.Decorators.Threading;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages.Connections;
using Xunit;

namespace AgentFramework.Core.Tests.Decorators
{
    public class ThreadingDecoratorTests
    {
        [Fact]
        public void CreatesNewThreadFromUnthreadedInboundMessage()
        {
            var inboundMessage = new ConnectionRequestMessage();

            var outboundMessage = inboundMessage.CreateThreadedReply<ConnectionResponseMessage>();

            var threadingBlock = outboundMessage.GetDecorator<ThreadDecorator>("thread");

            Assert.True(threadingBlock.ThreadId == inboundMessage.Id);
            Assert.True(threadingBlock.SenderOrder == 0);
            Assert.True(threadingBlock.RecievedOrders.Count == 0);
        }

        [Fact]
        public void AddsToThreadFromThreadedInboundMessage()
        {
            var inboundMessage = new ConnectionRequestMessage();

            var threadId = Guid.NewGuid().ToString();
            inboundMessage.AddDecorator(new ThreadDecorator()
            {
                ThreadId = threadId
            }, "thread");

            var outgoingMessage = inboundMessage.CreateThreadedReply<ConnectionResponseMessage>();

            var threadingBlock = outgoingMessage.GetDecorator<ThreadDecorator>("thread");

            Assert.True(threadingBlock.ThreadId == threadId);
            Assert.True(threadingBlock.SenderOrder == 0);
            Assert.True(threadingBlock.RecievedOrders.Count == 0);
        }

        [Fact]
        public void ThreadFromThrowsExceptionOnAlreadyThreadedMessage()
        {
            var message = new ConnectionRequestMessage();

            var threadId = Guid.NewGuid().ToString();
            message.AddDecorator(new ThreadDecorator()
            {
                ThreadId = threadId
            }, "thread");

            var inboundMessage = new ConnectionInvitationMessage();

            var ex = Assert.Throws<AgentFrameworkException>(() => message.ThreadFrom(inboundMessage));
            Assert.True(ex.ErrorCode == ErrorCode.InvalidMessage);
        }
    }
}
