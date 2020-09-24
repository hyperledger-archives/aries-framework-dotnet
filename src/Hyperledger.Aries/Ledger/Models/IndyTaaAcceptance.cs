namespace Hyperledger.Aries.Ledger
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
        /// The taa acceptance mechanism
        /// </summary>
        /// <value></value>
        public string AcceptanceMechanism { get; set; } = "service_agreement";

        /// <summary>
        /// The acceptance timestap in Unix Epoch time format
        /// </summary>
        /// <value></value>
        public long AcceptanceDate { get; set; }
    }
}
