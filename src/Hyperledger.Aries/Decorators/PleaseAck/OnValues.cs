namespace Hyperledger.Aries.Decorators.PleaseAck
{
    /// <summary>
    /// Represents the possible values in the "on" field of the please_ack decorator
    /// </summary>
    public enum OnValues
    {
        /// <summary>
        /// The RECEIPT acknowledgement is the easiest ack mechanism and requests that an ack is sent on receipt
        /// of the message. This way of requesting an ack is to verify whether the other agent successfully received
        /// the message. It implicitly means the agent was able to unpack the message.
        /// </summary>
        RECEIPT,

        /// <summary>
        /// The OUTCOME acknowledgement is the more advanced ack mechanism and requests that an ack is sent
        /// on outcome of the message. By default outcome means the message has been handled and processed without
        /// business logic playing a role in the decision.
        /// </summary>
        OUTCOME
    }
}
