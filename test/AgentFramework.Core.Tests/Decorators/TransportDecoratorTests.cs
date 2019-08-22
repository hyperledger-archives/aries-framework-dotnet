using AgentFramework.Core.Decorators.Transport;
using AgentFramework.Core.Messages.Connections;
using Xunit;

namespace AgentFramework.Core.Tests.Decorators
{
    public class TransportDecoratorTests
    {
        [Fact]
        public void CanAddTransportDecoratorToMessage()
        {
            var outboundMessage = new ConnectionRequestMessage();

            outboundMessage.AddReturnRouting();

            var transportDecorator = outboundMessage.GetDecorator<TransportDecorator>(Core.Decorators.Decorators.TransportDecorator);

            Assert.NotNull(transportDecorator);
            Assert.True(transportDecorator.ReturnRoute == ReturnRouteTypes.all.ToString("G"));
        }

        [Fact]
        public void CanDetectTransportDecoratorOnMessage()
        {
            var outboundMessage = new ConnectionRequestMessage();

            outboundMessage.AddReturnRouting();

            Assert.True(outboundMessage.ReturnRoutingRequested());
        }
    }
}
