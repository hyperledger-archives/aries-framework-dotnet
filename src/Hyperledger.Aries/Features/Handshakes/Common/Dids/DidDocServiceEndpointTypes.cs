namespace Hyperledger.Aries.Features.Handshakes.Common.Dids
{
    /// <summary>
    /// Did document service endpoint types.
    /// </summary>
    public static class DidDocServiceEndpointTypes
    {
        /// <summary>
        /// The indy agent.
        /// </summary>
        public const string IndyAgent = "IndyAgent";

        /// <summary>
        /// Did communication service endpoint
        /// https://github.com/hyperledger/aries-rfcs/blob/main/features/0067-didcomm-diddoc-conventions/README.md#service-conventions
        /// </summary>
        public const string DidCommunication = "did-communication";
    }
}
