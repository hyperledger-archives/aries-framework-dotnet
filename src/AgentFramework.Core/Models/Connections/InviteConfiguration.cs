using System.Collections.Generic;

namespace AgentFramework.Core.Models.Connections
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