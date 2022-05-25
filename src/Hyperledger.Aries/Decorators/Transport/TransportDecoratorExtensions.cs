using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Decorators;
using Hyperledger.Aries.Decorators.Transport;

namespace System
{
    /// <summary>
    /// Message threading extensions.
    /// </summary>
    public static class TransportDecoratorExtensions
    {
        /// <summary>
        /// Adds return routing to message
        /// </summary>
        /// <param name="message">The message to add return routing</param>
        /// <param name="returnType">The message to add return routing</param>
        public static void AddReturnRouting(this AgentMessage message, ReturnRouteTypes returnType = ReturnRouteTypes.all)
        {
            message.AddDecorator(new TransportDecorator
            {
                ReturnRoute = returnType.ToString("G")
            }, DecoratorNames.TransportDecorator);
        }

        /// <summary>
        /// Adds return routing to message
        /// </summary>
        /// <param name="message">The message to add return routing</param>
        public static bool ReturnRoutingRequested(this AgentMessage message)
        {
            try
            {
                var transportDecorator = message.FindDecorator<TransportDecorator>(DecoratorNames.TransportDecorator);

                if (transportDecorator != null)
                {
                    return true;
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds return routing to message
        /// </summary>
        /// <param name="message">The message to add return routing</param>
        public static bool ReturnRoutingRequested(this UnpackedMessageContext message)
        {
            try
            {
                var transportDecorator = message.FindDecorator<TransportDecorator>(DecoratorNames.TransportDecorator);

                if (transportDecorator != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
