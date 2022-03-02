using System;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Storage;

namespace Hyperledger.Aries
{
    /// <summary>
    /// Agent Framework exception
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AriesFrameworkException : Exception
    {
        /// <summary>
        /// Gets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public ErrorCode ErrorCode { get; }
   
        /// <summary>
        /// Gets the message context record.
        /// May be <code>null</code>.
        /// </summary>
        /// <value>
        /// The message context record. 
        /// </value>
        public RecordBase ContextRecord { get; }

        /// <summary>
        /// Gets the context Record ID.
        /// </summary>
        /// <value>
        /// The context record ID. 
        /// </value>
        public string ContextRecordId { get; }
        
        /// <summary>
        /// Gets the connection record.
        /// May be <code>null</code>.
        /// </summary>
        /// <value>
        /// The connection record. 
        /// </value>
        public ConnectionRecord ConnectionRecord { get; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public AriesFrameworkException(ErrorCode errorCode) : this(errorCode,
            $"Framework error occured. Code: {errorCode}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        public AriesFrameworkException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AriesFrameworkException(ErrorCode errorCode, string message, Exception innerException) : base(message,
            innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="messages">The message to concatenate together.</param>
        public AriesFrameworkException(ErrorCode errorCode, string[] messages) : base(string.Join("\n", messages))
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="contextRecord"></param>
        /// <param name="connectionRecord"></param>
        public AriesFrameworkException(ErrorCode errorCode, string message, RecordBase contextRecord, ConnectionRecord connectionRecord) :
            base(message)
        {
            ErrorCode = errorCode;
            ContextRecord = contextRecord;
            ConnectionRecord = connectionRecord;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AriesFrameworkException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="contextRecordId">The record ID</param>
        public AriesFrameworkException(ErrorCode errorCode, string message, string contextRecordId) :
            base(message)
        {
            ErrorCode = errorCode;
            ContextRecordId = contextRecordId;
        }
    }
}
