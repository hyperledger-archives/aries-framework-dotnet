using Newtonsoft.Json;

namespace Hyperledger.Aries.Decorators.PleaseAck
{
    /// <summary>
    /// Please ACK decorator for requesting an acknowledgement.
    ///
    /// Based on Aries RFC 0317: Please ACK Decorator
    /// https://github.com/hyperledger/aries-rfcs/blob/main/features/0317-please-ack/README.md
    /// </summary>
    public class PleaseAckDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseAckDecorator"/> class.
        /// </summary>
        /// <param name="onValuesArray">The On values of the please ack decorator.</param>
        public PleaseAckDecorator(OnValues[] onValuesArray)
        {
            On = onValuesArray;
        }

        /// <summary>
        /// The only field for the please ack decorator. Required array. Describes the circumstances under
        /// which an ack is desired. Possible values in this array include RECEIPT and OUTCOME.
        /// If both values are present, it means an acknowledgement is requested for both the receipt and outcome
        /// of the message
        /// </summary>
        /// <value>The on value</value>
        [JsonProperty("on", NullValueHandling = NullValueHandling.Ignore)]
        public OnValues[] On;
    }
}
