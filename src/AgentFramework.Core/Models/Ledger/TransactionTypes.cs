// ReSharper disable InconsistentNaming
namespace AgentFramework.Core.Models.Ledger
{
    /// <summary>
    /// Transaction types
    /// </summary>
    public static class TransactionTypes
    {
        /// <summary>
        /// The nym
        /// </summary>
        public const string NYM = "1";
        /// <summary>
        /// The attribute
        /// </summary>
        public const string ATTRIB = "100";
        /// <summary>
        /// The schema
        /// </summary>
        public const string SCHEMA = "101";
        /// <summary>
        /// The cred definition
        /// </summary>
        public const string CRED_DEF = "102";
        /// <summary>
        /// The revoc reg definition
        /// </summary>
        public const string REVOC_REG_DEF = "113";
        /// <summary>
        /// The revoc reg entry
        /// </summary>
        public const string REVOC_REG_ENTRY = "114";
        /// <summary>
        /// The xfer public
        /// </summary>
        public const string XFER_PUBLIC = "10001";
    }
}
