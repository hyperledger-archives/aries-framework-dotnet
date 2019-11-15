using AgentFramework.Core.Models.Ledger;

namespace AgentFramework.Core.Models
{
    /// <summary>
    /// Transaction Author Agreement Acceptance Model
    /// </summary>
    public class IndyTaaAcceptance : IndyTaa
    {
        /// <summary>
        /// The digest of the accepted text and version calculated as
        /// hex(sha256("version" + "text"))
        /// </summary>
        /// <value></value>
        public string Digest { get; set; }

        /// <summary>
        /// The acceptance timestap in Unix Epoch time format
        /// </summary>
        /// <value></value>
        public long AcceptanceDate { get; set; }
    }
}
