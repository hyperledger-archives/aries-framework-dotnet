using Hyperledger.Aries.Decorators.Transport;
using Hyperledger.Aries.Features.DidExchange;
using System;
using Xunit;

namespace Hyperledger.Aries.Tests.Decorators
{
    public class TransportDecoratorTests
    {
        [Fact]
        public void CanAddTransportDecoratorToMessage()
        {
            var outboundMessage = new ConnectionRequestMessage();

            outboundMessage.AddReturnRouting();

            var transportDecorator = outboundMessage.GetDecorator<TransportDecorator>(Aries.Decorators.DecoratorNames.TransportDecorator);

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
