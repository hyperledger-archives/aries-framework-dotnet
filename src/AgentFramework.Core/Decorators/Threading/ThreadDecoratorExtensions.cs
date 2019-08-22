using System;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Decorators.Threading
{
    /// <summary>
    /// Message threading extensions.
    /// </summary>
    public static class ThreadDecoratorExtensions
    {
        /// <summary>
        /// Threading decorator extension.
        /// </summary>
        public static string DecoratorIdentifier => "thread";

        /// <summary>
        /// Created a new threaded message response
        /// </summary>
        /// <param name="message">The message to thread from.</param>
        public static T CreateThreadedReply<T>(this AgentMessage message) where T : AgentMessage, new ()
        {
            var newMsg = new T();
            newMsg.ThreadMessage(message);
            return newMsg;
        }

        /// <summary>
        /// Threads the current message from a previous message.
        /// </summary>
        /// <param name="message">The message to add threading to.</param>
        /// <param name="previousMessage">The message to thread from.</param>
        public static void ThreadFrom(this AgentMessage message, AgentMessage previousMessage)
        {
            bool hasThreadBlock = false;
            try
            {
                message.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
                hasThreadBlock = true;
            }
            catch (AgentFrameworkException) { }

            if (hasThreadBlock)
                throw new AgentFrameworkException(ErrorCode.InvalidMessage, "Cannot thread message when it already has a valid thread decorator");

            message.ThreadMessage(previousMessage);
        }

        /// <summary>
        /// Gets the current messages thread id.
        /// </summary>
        /// <param name="message">Message to extract the thread id from.</param>
        /// <returns>Thread id of the message.</returns>
        public static string GetThreadId(this AgentMessage message)
        {
            string threadId = null;
            try
            {
                var threadBlock = message.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
                threadId = threadBlock.ThreadId;
            }
            catch (Exception)
            {
                // ignored
            }

            if (string.IsNullOrEmpty(threadId))
                threadId = message.Id;

            return threadId;
        }

        /// <summary>
        /// Threads the current message.
        /// </summary>
        /// <param name="messageToThread">Message to thread.</param>
        /// <param name="threadId">Thread id to thread the message with.</param>
        public static void ThreadFrom(this AgentMessage messageToThread, string threadId)
        {
            var currentThreadContext = new ThreadDecorator
            {
                ThreadId = threadId
            };
            messageToThread.AddDecorator(currentThreadContext, DecoratorIdentifier);
        }

        private static void ThreadMessage(this AgentMessage messageToThread, AgentMessage messageToThreadFrom)
        {
            ThreadDecorator previousMessageThreadContext = null;
            try
            {
                previousMessageThreadContext = messageToThreadFrom.GetDecorator<ThreadDecorator>(DecoratorIdentifier);
            }
            catch (AgentFrameworkException) { }

            ThreadDecorator currentThreadContext;
            if (previousMessageThreadContext != null)
            {
                currentThreadContext = new ThreadDecorator
                {
                    ParentThreadId = previousMessageThreadContext.ParentThreadId,
                    ThreadId = previousMessageThreadContext.ThreadId
                };
            }
            else
            {
                currentThreadContext = new ThreadDecorator
                {
                    ThreadId = messageToThreadFrom.Id
                };
            }


            messageToThread.AddDecorator(currentThreadContext, DecoratorIdentifier);
        }
    }
}
