namespace Hyperledger.Aries
{
    /// <summary>An error code that identifies the type of exception</summary>
    public enum ErrorCode
    {
        /// <summary>
        /// The wallet already provisioned
        /// </summary>
        WalletAlreadyProvisioned,
        /// <summary>
        /// The record not found
        /// </summary>
        RecordNotFound,
        /// <summary>
        /// The record in invalid state
        /// </summary>
        RecordInInvalidState,
        /// <summary>
        /// The ledger operation rejected
        /// </summary>
        LedgerOperationRejected,
        /// <summary>
        /// The route message error
        /// </summary>
        RouteMessageError,
        /// <summary>
        /// The a2 a message transmission error
        /// </summary>
        A2AMessageTransmissionError,
        /// <summary>
        /// The invalid message
        /// </summary>
        InvalidMessage,
        /// <summary>
        /// The message unpack error
        /// </summary>
        MessageUnpackError,
        /// <summary>
        /// The parameter was in an invalid format
        /// </summary>
        InvalidParameterFormat,
        /// <summary>
        /// Record has invalid or missing data
        /// </summary>
        InvalidRecordData,
        /// <summary>
        /// Insufficient funds at the specified address
        /// </summary>
        PaymentInsufficientFunds,
        /// <summary>
        /// Encoding values in the supplied proof are invalid
        /// </summary>
        InvalidProofEncoding,
        /// <summary>
        /// A given signature was invalid
        /// </summary>
        InvalidSignatureEncoding,
        /// <summary>
        /// The revocation registry unavailable
        /// </summary>
        RevocationRegistryUnavailable,
        /// <summary>
        /// The revocation interval is out of range
        /// </summary>
        RevocationIntervalOutOfRange,
        /// <summary>
        /// Item not found on ledger
        /// </summary>
        LedgerItemNotFound,
        /// <summary>
        /// No public did was provisioned
        /// </summary>
        NoPublicDid,
        /// <summary>
        /// The did method is not supported
        /// </summary>
        UnsupportedDidMethod,
        /// <summary>
        /// The pool does not exist
        /// </summary>
        PoolNotFound,
    }
}
