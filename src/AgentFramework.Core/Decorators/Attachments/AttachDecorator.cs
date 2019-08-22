using System.Collections.Generic;
using System.Linq;

namespace AgentFramework.Core.Decorators.Attachments
{
    /// <summary>
    /// Represents an attachment decorator <code>~attach</code>
    /// </summary>
    /// <seealso cref="Attachment" />
    public class AttachDecorator : List<Attachment>
    {
        /// <summary>
        /// Gets the <see cref="Attachment"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="Attachment"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public Attachment this[string name] => this.FirstOrDefault(x => x.Nickname == name);

        /// <summary>
        /// Gets the <see cref="Attachment"/> with the specified attachment.
        /// </summary>
        /// <value>
        /// The <see cref="Attachment"/>.
        /// </value>
        /// <param name="attachment">The attachment.</param>
        /// <returns></returns>
        public Attachment this[Attachment attachment] => this.FirstOrDefault(x => x.Nickname == attachment.Nickname);
    }
}
