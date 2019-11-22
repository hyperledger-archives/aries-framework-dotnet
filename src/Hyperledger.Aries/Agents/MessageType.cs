using System;
using System.Collections.Generic;
using Hyperledger.Aries.Utils;

namespace Hyperledger.Aries.Agents
{
    /// <summary>
    /// A representaiton of a message type.
    /// </summary>
    public sealed class MessageType : IEquatable<MessageType>
    {
        /// <summary>
        /// Constructor for creating the representation from a message type uri.
        /// </summary>
        /// <param name="messageTypeUri">Message type uri.</param>
        public MessageType(string messageTypeUri)
        {
            var (uri, messageFamilyName, messageVersion, messageName) = MessageUtils.DecodeMessageTypeUri(messageTypeUri);

            BaseUri = uri;
            MessageFamilyName = messageFamilyName;
            MessageVersion = messageVersion;
            MessageName = messageName;
            MessageTypeUri = messageTypeUri;
        }

        /// <summary>
        /// Create a <see cref="MessageType"/> instance from a string Uri
        /// </summary>
        /// <returns>The URI.</returns>
        /// <param name="messageTypeUri">Message type URI.</param>
        public static MessageType FromUri(string messageTypeUri) => new MessageType(messageTypeUri);

        /// <summary>
        /// Base uri the message type derives from.
        /// </summary>
        public string BaseUri { get; }

        /// <summary>
        /// Message family the message belongs to.
        /// </summary>
        public string MessageFamilyName { get; }

        /// <summary>
        /// Message family uri the message belongs to.
        /// </summary>
        public string MessageFamilyUri => $"{BaseUri}/{MessageFamilyName}/{MessageVersion}";

        /// <summary>
        /// Message version the message belongs to.
        /// </summary>
        public string MessageVersion { get; }

        /// <summary>
        /// Message name of the message.
        /// </summary>
        public string MessageName { get; }

        /// <summary>
        /// Full Uri of the message type.
        /// </summary>
        public string MessageTypeUri { get; }

        /// <summary>
        /// Implcit operator that converts a string to MessageType
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator MessageType(string value)
        {
            return new MessageType(value);
        }

        #region Equality methods

        /// <summary>
        /// Determines whether a specified instance of <see cref="MessageType"/> is equal
        /// to another specified <see cref="string"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="MessageType"/> to compare.</param>
        /// <param name="rhs">The second <see cref="string"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MessageType lhs, string rhs) => lhs.MessageTypeUri == rhs;

        /// <summary>
        /// Determines whether a specified instance of <see cref="MessageType"/> is not
        /// equal to another specified <see cref="string"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="MessageType"/> to compare.</param>
        /// <param name="rhs">The second <see cref="string"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MessageType lhs, string rhs) => lhs.MessageTypeUri != rhs;

        /// <summary>
        /// Determines whether a specified instance of <see cref="string"/> is equal to another specified <see cref="MessageType"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="string"/> to compare.</param>
        /// <param name="rhs">The second <see cref="MessageType"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(string lhs, MessageType rhs) => rhs.MessageTypeUri == lhs;

        /// <summary>
        /// Determines whether a specified instance of <see cref="string"/> is not equal to another specified <see cref="MessageType"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="string"/> to compare.</param>
        /// <param name="rhs">The second <see cref="MessageType"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(string lhs, MessageType rhs) => rhs.MessageTypeUri != lhs;

        /// <summary>
        /// Determines whether a specified instance of <see cref="MessageType"/> is equal
        /// to another specified <see cref="MessageType"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="MessageType"/> to compare.</param>
        /// <param name="rhs">The second <see cref="MessageType"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MessageType lhs, MessageType rhs) => lhs.MessageTypeUri == rhs.MessageTypeUri;

        /// <summary>
        /// Determines whether a specified instance of <see cref="MessageType"/> is not
        /// equal to another specified <see cref="MessageType"/>.
        /// </summary>
        /// <param name="lhs">The first <see cref="MessageType"/> to compare.</param>
        /// <param name="rhs">The second <see cref="MessageType"/> to compare.</param>
        /// <returns><c>true</c> if <c>lhs</c> and <c>rhs</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MessageType lhs, MessageType rhs) => lhs.MessageTypeUri != rhs.MessageTypeUri;

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:AgentFramework.Core.Messages.MessageType"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:AgentFramework.Core.Messages.MessageType"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current
        /// <see cref="T:AgentFramework.Core.Messages.MessageType"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => Equals(obj as MessageType);

        /// <summary>
        /// Determines whether the specified <see cref="MessageType"/> is equal to the
        /// current <see cref="T:AgentFramework.Core.Messages.MessageType"/>.
        /// </summary>
        /// <param name="other">The <see cref="MessageType"/> to compare with the current <see cref="T:AgentFramework.Core.Messages.MessageType"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MessageType"/> is equal to the current
        /// <see cref="T:AgentFramework.Core.Messages.MessageType"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(MessageType other) => !(other is null) && MessageTypeUri == other.MessageTypeUri;

        /// <summary>
        /// Serves as a hash function for a <see cref="T:AgentFramework.Core.Messages.MessageType"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => 293836374 + EqualityComparer<string>.Default.GetHashCode(MessageTypeUri);

        #endregion
    }
}
