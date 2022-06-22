using System.Collections.Generic;
using Hyperledger.Aries.Features.Handshakes.Common;

namespace Hyperledger.Aries.Features.Handshakes.Connection.Models
{
    /// <summary>
    /// Config for controlling invitation creation.
    /// </summary>
    public class InviteConfiguration
    {
        /// <summary>
        /// Id of the resulting connection record created
        /// by the invite.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// Used to generated an invitation that multiple parties
        /// can use to connect.
        /// </summary>
        public bool MultiPartyInvitation { get; set; }

        /// <summary>
        /// Alias object for marking the invite subject
        /// with an alias for giving the inviter greater context. 
        /// </summary>
        public ConnectionAlias TheirAlias { get; set; } = new ConnectionAlias();

        /// <summary>
        /// For optionally setting my alias information
        /// on the invite.
        /// </summary>
        public ConnectionAlias MyAlias { get; set; } = new ConnectionAlias();

        /// <summary>
        /// For automatically accepting a
        /// connection request generated from this created invite
        /// </summary>
        public bool AutoAcceptConnection { get; set; }

        /// <summary>
        /// Indicator if this invitation/connection record should use the did:key exchange format
        /// </summary>
        public bool UseDidKeyFormat { get; set; } = false;

        /// <summary>
        /// Indicator if this invitation should keep the 
        /// </summary>
        public bool UsePublicDid { get; set; } = false;

        /// <summary>
        /// Controls the tags that are persisted against the invite/connection record.
        /// </summary>
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();

        /// <inheritdoc />
        public override string ToString() =>
            $"{GetType().Name}: " +
            $"ConnectionId={ConnectionId}, " +
            $"MultiPartyInvitation={MultiPartyInvitation}, " +
            $"TheirAlias={TheirAlias}, " +
            $"MyAlias={MyAlias}, " +
            $"AutoAcceptConnection={AutoAcceptConnection}, " +
            $"Tags={string.Join(",", Tags ?? new Dictionary<string, string>())}";
    }
}
