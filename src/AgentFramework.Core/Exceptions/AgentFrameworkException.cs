using System;

namespace AgentFramework.Core.Exceptions
{
    /// <summary>
    /// Agent Framework exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AgentFrameworkException : Exception
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public AgentFrameworkException(ErrorCode errorCode) : this(errorCode,
            $"Framework error occured. Code: {errorCode}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public AgentFrameworkException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AgentFrameworkException(ErrorCode errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="messages">The message to concatenate together.</param>
        public AgentFrameworkException(ErrorCode errorCode, string[] messages) : base(string.Join("\n",messages))
        {
            ErrorCode = errorCode;
        }
    }
}
