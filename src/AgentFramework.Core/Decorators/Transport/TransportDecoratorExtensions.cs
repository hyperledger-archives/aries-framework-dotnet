using System;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Decorators.Transport
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
        public static void AddReturnRouting(this AgentMessage message)
        {
            message.AddDecorator(new TransportDecorator
            {
                ReturnRoute = ReturnRouteTypes.all.ToString("G")
            }, Decorators.TransportDecorator);
        }

        /// <summary>
        /// Adds return routing to message
        /// </summary>
        /// <param name="message">The message to add return routing</param>
        public static bool ReturnRoutingRequested(this AgentMessage message)
        {
            try
            {
                var transportDecorator = message.FindDecorator<TransportDecorator>(Decorators.TransportDecorator);

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
        public static bool ReturnRoutingRequested(this MessageContext message)
        {
            try
            {
                var transportDecorator = message.FindDecorator<TransportDecorator>(Decorators.TransportDecorator);

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
