using System.Runtime.Serialization;

namespace Hyperledger.Aries.Features.Handshakes
{
    /// <summary> Current supported handshake protocols </summary>
    public enum HandshakeProtocol
    {
        /// <summary> The connections protocol </summary>
        [EnumMember(Value = HandshakeProtocolUri.Connections)]
        Connections,
        /// <summary> The did exchange protocol </summary>
        [EnumMember(Value = HandshakeProtocolUri.DidExchange)]
        DidExchange
    }
}
