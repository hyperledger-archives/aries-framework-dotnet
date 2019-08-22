using System;
using AgentFramework.Core.Messages;

namespace AgentFramework.Core.Decorators.Attachments
{
    /// <summary>
    /// Attachment extensions
    /// </summary>
    public static class AttachExtensions
    {
        /// <summary>
        /// Adds an attachment to the message using the ~attach attachment
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="attachment">The attachment.</param>
        /// <param name="overrideExisting">if set to <c>true</c> [override existing].</param>
        public static void AddAttachment(this AgentMessage message, Attachment attachment, bool overrideExisting = true)
        {
            var decorator = message.FindDecorator<AttachDecorator>("attach") ?? new AttachDecorator();
            var existing = decorator[attachment];

            if (existing != null && !overrideExisting)
            {
                throw new ArgumentException($"Attachment {attachment.Nickname} already exists.");
            }
            if (existing != null)
            {
                decorator.Remove(existing);
            }
            decorator.Add(attachment);
            message.AddDecorator(decorator, "attach");
        }

        /// <summary>
        /// Gets the attachment.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nickname">The nickname.</param>
        public static  Attachment GetAttachment(this AgentMessage message, string nickname)
        {
            var decorator = message.FindDecorator<AttachDecorator>("attach") ?? new AttachDecorator();
            return decorator[nickname];
        }

        /// <summary>
        /// Removes the attachment.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="nickname">The nickname.</param>
        /// <returns></returns>
        public static bool RemoveAttachment(this AgentMessage message, string nickname)
        {
            var decorator = message.FindDecorator<AttachDecorator>("attach") ?? new AttachDecorator();
            var result = decorator.Remove(decorator[nickname]);
            message.SetDecorator(decorator, "attach");

            return result;
        }
    }
}
